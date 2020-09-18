using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using Crestron;
using Crestron.Logos.SplusLibrary;
using Crestron.Logos.SplusObjects;
using Crestron.SimplSharp;
using QscQsys;
using Crestron.SimplSharp.SimplSharpExtensions;
using TCP_Client;

namespace UserModule_QSYS_SNAPSHOT_CONTROLLER
{
    public class UserModuleClass_QSYS_SNAPSHOT_CONTROLLER : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        Crestron.Logos.SplusObjects.DigitalInput LOAD;
        Crestron.Logos.SplusObjects.DigitalInput SAVE;
        Crestron.Logos.SplusObjects.AnalogInput NUMBER;
        StringParameter COREID;
        StringParameter COMPONENTNAME;
        UShortParameter MODE;
        QscQsys.QsysSnapshot SNAPSHOT;
        object NUMBER_OnChange_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                ushort X = 0;
                
                
                __context__.SourceCodeLine = 22;
                if ( Functions.TestForTrue  ( ( Functions.BoolToInt (MODE  .Value == 1))  ) ) 
                    { 
                    __context__.SourceCodeLine = 24;
                    while ( Functions.TestForTrue  ( ( Functions.BoolToInt (X != NUMBER  .UshortValue))  ) ) 
                        { 
                        __context__.SourceCodeLine = 26;
                        X = (ushort) ( NUMBER  .UshortValue ) ; 
                        __context__.SourceCodeLine = 28;
                        if ( Functions.TestForTrue  ( ( LOAD  .Value)  ) ) 
                            { 
                            __context__.SourceCodeLine = 30;
                            SNAPSHOT . LoadSnapshot ( (ushort)( X )) ; 
                            } 
                        
                        else 
                            {
                            __context__.SourceCodeLine = 32;
                            if ( Functions.TestForTrue  ( ( SAVE  .Value)  ) ) 
                                { 
                                __context__.SourceCodeLine = 34;
                                SNAPSHOT . SaveSnapshot ( (ushort)( X )) ; 
                                } 
                            
                            }
                        
                        __context__.SourceCodeLine = 24;
                        } 
                    
                    } 
                
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    object LOAD_OnPush_1 ( Object __EventInfo__ )
    
        { 
        Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
        try
        {
            SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
            
            __context__.SourceCodeLine = 42;
            if ( Functions.TestForTrue  ( ( Functions.BoolToInt (MODE  .Value == 2))  ) ) 
                { 
                __context__.SourceCodeLine = 44;
                SNAPSHOT . LoadSnapshot ( (ushort)( NUMBER  .UshortValue )) ; 
                } 
            
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
object SAVE_OnPush_2 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 50;
        if ( Functions.TestForTrue  ( ( Functions.BoolToInt (MODE  .Value == 2))  ) ) 
            { 
            __context__.SourceCodeLine = 52;
            SNAPSHOT . SaveSnapshot ( (ushort)( NUMBER  .UshortValue )) ; 
            } 
        
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

public override object FunctionMain (  object __obj__ ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusFunctionMainStartCode();
        
        __context__.SourceCodeLine = 58;
        SNAPSHOT . Initialize ( COREID  .ToString(), COMPONENTNAME  .ToString()) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    return __obj__;
    }
    

public override void LogosSplusInitialize()
{
    _SplusNVRAM = new SplusNVRAM( this );
    
    LOAD = new Crestron.Logos.SplusObjects.DigitalInput( LOAD__DigitalInput__, this );
    m_DigitalInputList.Add( LOAD__DigitalInput__, LOAD );
    
    SAVE = new Crestron.Logos.SplusObjects.DigitalInput( SAVE__DigitalInput__, this );
    m_DigitalInputList.Add( SAVE__DigitalInput__, SAVE );
    
    NUMBER = new Crestron.Logos.SplusObjects.AnalogInput( NUMBER__AnalogSerialInput__, this );
    m_AnalogInputList.Add( NUMBER__AnalogSerialInput__, NUMBER );
    
    MODE = new UShortParameter( MODE__Parameter__, this );
    m_ParameterList.Add( MODE__Parameter__, MODE );
    
    COREID = new StringParameter( COREID__Parameter__, this );
    m_ParameterList.Add( COREID__Parameter__, COREID );
    
    COMPONENTNAME = new StringParameter( COMPONENTNAME__Parameter__, this );
    m_ParameterList.Add( COMPONENTNAME__Parameter__, COMPONENTNAME );
    
    
    NUMBER.OnAnalogChange.Add( new InputChangeHandlerWrapper( NUMBER_OnChange_0, true ) );
    LOAD.OnDigitalPush.Add( new InputChangeHandlerWrapper( LOAD_OnPush_1, false ) );
    SAVE.OnDigitalPush.Add( new InputChangeHandlerWrapper( SAVE_OnPush_2, false ) );
    
    _SplusNVRAM.PopulateCustomAttributeList( true );
    
    NVRAM = _SplusNVRAM;
    
}

public override void LogosSimplSharpInitialize()
{
    SNAPSHOT  = new QscQsys.QsysSnapshot();
    
    
}

public UserModuleClass_QSYS_SNAPSHOT_CONTROLLER ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}




const uint LOAD__DigitalInput__ = 0;
const uint SAVE__DigitalInput__ = 1;
const uint NUMBER__AnalogSerialInput__ = 0;
const uint COREID__Parameter__ = 10;
const uint COMPONENTNAME__Parameter__ = 11;
const uint MODE__Parameter__ = 12;

[SplusStructAttribute(-1, true, false)]
public class SplusNVRAM : SplusStructureBase
{

    public SplusNVRAM( SplusObject __caller__ ) : base( __caller__ ) {}
    
    
}

SplusNVRAM _SplusNVRAM = null;

public class __CEvent__ : CEvent
{
    public __CEvent__() {}
    public void Close() { base.Close(); }
    public int Reset() { return base.Reset() ? 1 : 0; }
    public int Set() { return base.Set() ? 1 : 0; }
    public int Wait( int timeOutInMs ) { return base.Wait( timeOutInMs ) ? 1 : 0; }
}
public class __CMutex__ : CMutex
{
    public __CMutex__() {}
    public void Close() { base.Close(); }
    public void ReleaseMutex() { base.ReleaseMutex(); }
    public int WaitForMutex() { return base.WaitForMutex() ? 1 : 0; }
}
 public int IsNull( object obj ){ return (obj == null) ? 1 : 0; }
}


}
