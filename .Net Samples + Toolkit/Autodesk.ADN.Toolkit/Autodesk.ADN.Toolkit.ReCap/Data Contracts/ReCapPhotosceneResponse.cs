using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.ADN.Toolkit.ReCap.DataContracts
{
    /////////////////////////////////////////////////////////////////////////////////
    // ReCap Photoscene Response
    //
    /////////////////////////////////////////////////////////////////////////////////
    public class ReCapPhotosceneResponse : ReCapResponseBase
    {
        public ReCapPhotoscene Photoscene
        {
            get;
            set;
        }
    }
}
