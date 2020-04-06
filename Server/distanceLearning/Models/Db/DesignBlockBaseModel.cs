using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;

namespace Models.Db
{
    // 
    /// <summary>
    ///     ''' This model provides the common fields for all Design Blocks.
    ///     ''' </summary>
    public class DesignBlockBaseModel
    {
        // 
        // ====================================================================================================
        // -- instance properties
        // 
        public string backgroundImageFilename { get; set; }
        public int fontStyleId { get; set; }
        public int themeStyleId { get; set; }
        public bool padTop { get; set; }
        public bool padBottom { get; set; }
        public bool padRight { get; set; }
        public bool padLeft { get; set; }
        public string styleheight { get; set; }
        public bool asFullBleed { get; set; }
        public string btnStyleSelector { get; set; }
    }
}