using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace BehaviorDesigner.Runtime.Tasks{

public class EnchantedConditional : BehaviorDesigner.Runtime.Tasks.Conditional
{

   public enum LogLevel {ERROR, WARN, INFO}

    #region Log Methods

    protected void CheckDependencyComponentMissing(Component component, string compName, LogLevel logLevel = LogLevel.ERROR, bool? overrideCondition = null){
        if(notExist(component)){
            writeInConsole($"componente {compName} non trovato dallo script {this.GetType().Name} nel gameObject {gameObject.name}", logLevel);
        }
    }

    /// <summary>
    /// Controlla se è stata settata la proprietà, e emette un messaggio di errore
    /// Se la proprietà è object, verrà controllato prop == null
    /// Se la proprietà è string, verrà controllato string.isNullOrEmpty(prop)
    /// Se la proprietà è array o list, verrà controllato prop == null || prop.lenght == 0
    /// Se la proprietà è int o float verrà controllato prop == 0
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="propName"></param>
    /// <param name="logLevel"></param>
    protected void CheckDependencyPropMissing(object prop, string propName, LogLevel logLevel = LogLevel.ERROR, bool? overrideCondition = null){

        if((overrideCondition != null && (bool)overrideCondition) || (overrideCondition == null && notExist(prop))){
            writeInConsole($"proprietà {propName} non impostata nello script {this.GetType().Name} nel gameObject {gameObject.name}", logLevel);
        }
    }
    protected void CheckDependencyChildComponentMissing(Component component, string compName, LogLevel logLevel = LogLevel.ERROR, bool? overrideCondition = null){
        if((overrideCondition != null && (bool)overrideCondition) || (overrideCondition == null && notExist(component))){
            writeInConsole($"componente {compName} non trovato dallo script {this.GetType().Name} in un oggetto figlio del gameObject {gameObject.name}", logLevel);
        }
    }
    protected void CheckDependencyChildGameObjectMissing(GameObject checkGameObject, string gameObjectName, LogLevel logLevel = LogLevel.ERROR, bool? overrideCondition = null){
        if((overrideCondition != null && (bool)overrideCondition) || (overrideCondition == null && notExist(checkGameObject))){
            writeInConsole($"gameObject {gameObjectName} non trovato dallo script {this.GetType().Name} tra i figli del gameObject {gameObject.name}", logLevel);
        }
    }
    protected void CheckDependencyParentComponentMissing(Component component, string compName, LogLevel logLevel = LogLevel.ERROR, bool? overrideCondition = null){
        if(((overrideCondition != null && (bool)overrideCondition) || (overrideCondition == null && notExist(component)))){
            writeInConsole($"componente {compName} non trovato dallo script {this.GetType().Name} in un oggetto padre del gameObject {gameObject.name}", logLevel);
        }
    }

    protected void CheckDependencyMissing(GameObject obj, string customMessage, LogLevel logLevel = LogLevel.ERROR){
        if(notExist(obj)){
            writeInConsole($"dipendenza mancante nel gameObject {gameObject.name}; Errore: {customMessage}", logLevel);
        }
    }
    protected void CheckDependencyMissing(Component component, string customMessage, LogLevel logLevel = LogLevel.ERROR){
        if(notExist(component)){
            writeInConsole($"dipendenza mancante nel gameObject {gameObject.name}; Errore: {customMessage}", logLevel);
        }
    }


    private bool notExist(object obj){
            if(obj is string){
                return string.IsNullOrEmpty((string)obj);
            }
            else if(obj is List<object>){
                return (obj.Equals(null) || ((List<object>)obj).Count <= 0);
            }
            else if(obj is Array){
                return (obj.Equals(null) || ((Array)obj).Length <= 0);
            }
            else if(obj is float){
                return (float)obj == 0;
            }
            else if(obj is int){
                return (int)obj == 0;
            }

            else if(obj is LayerMask){
                return (LayerMask)obj == 0;
            }

            else{
                return obj == null || obj.Equals(null);
            }
    }

    private void writeInConsole(string text, LogLevel logLevel){
        switch(logLevel){
            case LogLevel.ERROR:
                Debug.LogError("Errore: " + text);
            break;
            case LogLevel.WARN:
                Debug.LogWarning("Attenzione: " + text);
            break;
            case LogLevel.INFO:
                Debug.Log("Info: " + text);
            break;
        }
    }

    #endregion


    #region Delay

    protected void DelayExecutionSeconds(float timeToWait, System.Action actionToRun, bool realTime = false){
        StartCoroutine(delayExecutionSecondsCorutine(timeToWait, actionToRun, realTime));
    }

    private IEnumerator delayExecutionSecondsCorutine(float timeToWait, System.Action actionToRun, bool realTime){
        if(realTime){
            yield return new WaitForSecondsRealtime(timeToWait);
        }
        else{
            yield return new WaitForSeconds(timeToWait);
        }
        actionToRun();
    }

    protected void DelayExecutionEndFrame(System.Action actionToRun){
        StartCoroutine(delayExecutionEndFrameCorutine(actionToRun));
    }

    private IEnumerator delayExecutionEndFrameCorutine(System.Action actionToRun){
        yield return new WaitForEndOfFrame();
        actionToRun();
    }
    protected void DelayExecutionFixedUpdate(System.Action actionToRun){
        StartCoroutine(delayExecutionFixedUpdateCorutine(actionToRun));
    }

    private IEnumerator delayExecutionFixedUpdateCorutine(System.Action actionToRun){
        yield return new WaitForEndOfFrame();
        actionToRun();
    }

    #endregion



    #region  Various
    protected bool IsLayerInLayermask(int layer, LayerMask layerMask){
        return layerMask == (layerMask | (1 << layer));
    }


    /// <summary>
    /// Trova il primo gameObject figlio con il nome specificato
    /// </summary>
    /// <param name="name">nome da cercare</param>
    /// <returns></returns>
    protected GameObject findChildByName(string name){
        foreach(Transform child in transform){
            if(child.name == name){
                return child.gameObject;
            }
        }
        return null;
    }
    /// <summary>
    /// Trova il primo gameObject figlio con il nome specificato, cercando nel gameObject padre specificato
    /// </summary>
    /// <param name="name">nome da cercare</param>
    /// <returns></returns>
    protected GameObject findChildByName(GameObject parent, string name){
        foreach(Transform child in parent.transform){
            if(child.name == name){
                return child.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Trova tutti i gameobject figli con il nome specificato
    /// </summary>
    /// <param name="name">Nome da cercare</param>
    /// <returns></returns>
    protected List<GameObject> findChildrenByName(string name){
        List<GameObject> retList = new List<GameObject>();
        foreach(Transform child in transform){
            if(child.name == name){
                retList.Add(child.gameObject);
            }
        }
        return retList;
    }
    /// <summary>
    /// Trova tutti i gameobject figli con il nome specificato cercando nel gameObject padre specificato
    /// </summary>
    /// <param name="name">Nome da cercare</param>
    /// <returns></returns>
    protected List<GameObject> findChildrenByName(GameObject parent, string name){
        List<GameObject> retList = new List<GameObject>();
        foreach(Transform child in parent.transform){
            if(child.name == name){
                retList.Add(child.gameObject);
            }
        }
        return retList;
    }


    #endregion
    
    
}
}