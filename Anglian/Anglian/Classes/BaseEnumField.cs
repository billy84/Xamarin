using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Anglian.Classes
{
    public class BaseEnumField
    {
        public ObservableCollection<BaseEnumValue> BaseEnums;

        public string FieldName;

        public System.DateTime LastUpdate;

        public string TableName;
    }
}
