﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace QscQsys.Intermediaries
{
    /// <summary>
    /// Acts as an intermediary between the QSys Core and the QsysNamedControls
    /// </summary>
    public sealed class NamedComponent : IQsysIntermediary
    {
        #region Fields

        private readonly string _name;
        private readonly QsysCore _core;
        private readonly Dictionary<string, NamedComponentControl> _controls;
        private readonly Dictionary<string, Action<QsysStateData>>  _controlUpdateCallbacks; 
        private bool _subscribe;

        #endregion

        #region Events

        public event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;

        public event EventHandler<ComponentControlEventArgs> OnComponentControlAdded;

        public event EventHandler<ComponentControlSubscribeEventArgs> OnComponentSubscribeChanged;

        #endregion

        #region Properties

        public string Name { get { return _name; } }

        public QsysCore Core { get { return _core; } }

        public bool Subscribe { get { return _subscribe; } }

        #endregion

        #region Constructor

        private NamedComponent(string name, QsysCore core)
        {
            _subscribe = true;
            _controls = new Dictionary<string, NamedComponentControl>();
            _controlUpdateCallbacks = new Dictionary<string, Action<QsysStateData>>();
            _name = name;
            _core = core;
        }

        #endregion

        #region Methods

        private void UpdateState(QsysStateData state)
        {
            var handler = OnFeedbackReceived;
            if (handler != null)
                handler(this, new QsysInternalEventsArgs(state));

            Action<QsysStateData> updateCallback;
            if (TryGetComponentUpdateCallback(state.Name, out updateCallback))
                updateCallback(state);
        }

        public Component ToComponentSubscribeControls()
        {
            return Component.Instantiate(
                                         Name,
                                         GetComponentControlsSubscribe().Select(control => control.ToControlName())
                );
        }

        #endregion

        #region SendData

        internal void SendChangePosition(string method, double position)
        {
            var change = new ComponentChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "position",
                        Caller = Name,
                        Method = method,
                        Position = position
                    }),
                Params =
                    new ComponentChangeParams()
                    {
                        Name = Name,
                        Controls =
                            new List<ComponentSetValue>() { new ComponentSetValue() { Name = method, Position = position } }
                    }
            };

            Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        internal void SendChangeDoubleValue(string method, double value)
        {
            var change = new ComponentChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "value",
                        Caller = Name,
                        Method = method,
                        Value = value,
                        StringValue = value.ToString()
                    }),
                Params =
                    new ComponentChangeParams()
                    {
                        Name = Name,
                        Controls =
                            new List<ComponentSetValue>() { new ComponentSetValue() { Name = method, Value = value } }
                    }
            };

            Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        internal void SendChangeStringValue(string method, string value)
        {
            var change = new ComponentChangeString()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "string_value",
                        Caller = Name,
                        Method = method,
                        StringValue = value
                    }),
                Params =
                    new ComponentChangeParamsString()
                    {
                        Name = Name,
                        Controls =
                            new List<ComponentSetValueString>()
                            {
                                new ComponentSetValueString() {Name = method, Value = value}
                            }
                    }
            };

            Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        #endregion

        #region ComponentControls

        public NamedComponentControl LazyLoadComponentControl(string name)
        {
            return LazyLoadComponentControl(name, true);
        }

        public NamedComponentControl LazyLoadComponentControl(string name, bool subscribe)
        {
            NamedComponentControl control;

            lock (_controls)
            {
                if (_controls.TryGetValue(name, out control))
                {
                    if (subscribe)
                        control.SetSubscribe();
                    return control;
                }

                Action<QsysStateData> updateCallback;
                control = NamedComponentControl.Create(name, this, subscribe, out updateCallback);
                _controls.Add(name, control);
                _controlUpdateCallbacks.Add(name, updateCallback);
            }

            SubscribeControl(control);

            var handler = OnComponentControlAdded;
            if (handler != null)
                handler(this, new ComponentControlEventArgs(control));

            return control;
        }

        public bool TryGetComponentControl(string name, out NamedComponentControl control)
        {
            lock (_controls)
            {
                return _controls.TryGetValue(name, out control);
            }
        }

        private bool TryGetComponentUpdateCallback(string name, out Action<QsysStateData> updateCallback)
        {
            lock (_controls)
            {
                return _controlUpdateCallbacks.TryGetValue(name, out updateCallback);
            }
        }

        public IEnumerable<NamedComponentControl> GetComponentControls()
        {
            lock (_controls)
            {
                return _controls.Values.ToArray();
            }
        }

        public IEnumerable<NamedComponentControl> GetComponentControlsSubscribe()
        {
            lock (_controls)
            {
                return _controls.Values.Where(c => c.Subscribe).ToArray();
            }
        } 

        #endregion

        #region Control Callbacks

        private void SubscribeControl(NamedComponentControl control)
        {
            if (control == null)
                return;

            control.OnSubscribeChanged += ControlOnSubscribeChanged;
        }

        private void UnsubscribeControl(NamedComponentControl control)
        {
            if (control == null)
                return;

            control.OnSubscribeChanged -= ControlOnSubscribeChanged;
        }

        private void ControlOnSubscribeChanged(object sender, BoolEventArgs args)
        {
            var control = sender as NamedComponentControl;
            
            if (control == null)
                return;

            var handler = OnComponentSubscribeChanged;
            if (handler != null)
                handler(this, new ComponentControlSubscribeEventArgs(control, args.Data));
        }

        #endregion

        #region Static Methods

        public static NamedComponent Create(string name, QsysCore core, out Action<QsysStateData> updateCallback)
        {
            var component = new NamedComponent(name, core);
            updateCallback = component.UpdateState;
            return component;
        }

        #endregion
    }

}