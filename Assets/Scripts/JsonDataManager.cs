using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonDataManager
{
    public const string getOdometerValJSON =

    @"{""operation:"" : ""getCurrentOdometer""}";

    public const string getRandomStatusJSON =
        @"{""operation"": ""getRandomStatus""}";
 
    public void GetValueOfRsponse()
    {

    }

    public class OdometerValueResp
    {
        public string operation;
        public float value;
        
        void OdometerValueRsp(string Text)
        {            
            OdometerValueResp resp = JsonUtility.FromJson<OdometerValueResp>(Text);
        }
    }
}
