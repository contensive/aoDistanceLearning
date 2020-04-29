
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizResultMessageModel : DbBaseModel {
        //
        public static readonly DbBaseTableMetadataModel tableMetadata = new DbBaseTableMetadataModel("Quiz Result Messages", "quizResultMessages", "default", false);
        //
        // -- instance properties
        public int QuizID;
        public int pointThreshold;
        public string copy;
    }
}

