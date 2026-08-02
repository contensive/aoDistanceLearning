using System;
using System.Collections.Generic;

namespace Contensive.distanceLearningImportTool.Model.architectureModels {
    // 
    [Serializable()]
    public class remoteResponseObject {
        public object data = new object();
        public List<errorClass> errors = new List<errorClass>();
    }
    // 
}