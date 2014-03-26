using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            private set;
        }

        [JsonConstructor]
        public ReCapPhotosceneResponse(
           ReCapPhotoscene photoscene)
        {
            Photoscene = photoscene;
        }

        public ReCapPhotosceneResponse(
           ReCapPhotoscene photoscene,
            string usage,
            string resource,
            ReCapError error):
            base(usage, resource, error)
        {
            Photoscene = photoscene;
        }

        public ReCapPhotosceneResponse()
        {

        }
    }
}
