

using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models
{
    public class DesignBlockFontModel : DesignBlockBaseModel {
        // 
        // ====================================================================================================
        //
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Design Block Fonts", "dbFonts", "default", false);        //
        //
    }
}
