

using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models {
    public class DesignBlockThemeModel : DesignBlockBaseModel {
        // 
        // ====================================================================================================
        //
        public static  DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Design Block Themes", "dbThemes", "default", false);        //
        //
    }
}

