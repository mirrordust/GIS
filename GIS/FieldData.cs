using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIS
{
    class FieldData
    {
        private string fieldName;
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName = value;
            }
        }

        private List<object> fieldData = new List<object>();
        public List<object> Values
        {
            get
            {
                return fieldData;
            }
            set
            {
                fieldData = value;
            }
        }

        public int Count
        {
            get
            {
                return fieldData.Count;
            }
        }
    }
}
