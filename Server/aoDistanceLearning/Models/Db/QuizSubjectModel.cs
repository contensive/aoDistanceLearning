﻿
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizSubjectModel : DbBaseModel {
        //
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Quiz Result Messages", "quizResultMessages", "default", false);
        public int quizId { get; set; }

    }
}

