using System.ComponentModel;


namespace PTLab10___WPF_LINQ
{
    public class BindingListEx<T> : BindingList<T>
    {
        public BindingListEx()
        {
            this.Add(default);
        }

        public BindingListEx(IEnumerable<T> data)
        {
            foreach (var item in data)
            {
                this.Add(item);
            }
        }


        protected override
        bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override
        void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            if(prop.PropertyType.GetInterface("IComparable") != null)
            {
                var itemsList = Items as List<T>;
                if(itemsList != null)
                {
                    Comparison<T> comparer = (x, y) =>
                    {
                        var val1 = prop.GetValue(x) as IComparable;
                        var val2 = prop.GetValue(y) as IComparable;
                        if (val1 != null && val2 != null)
                        {
                            return direction == ListSortDirection.Ascending ? val1.CompareTo(val2) : val2.CompareTo(val1);
                        }
                        return 0;
                    };
                    itemsList.Sort(comparer);
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }
       
        protected override
        bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override
        int FindCore(PropertyDescriptor prop, object key)
        {
            var type = prop.PropertyType;
            if((type == typeof(string) || type == typeof(int)) && key != null)
            {
                var itemsList = Items as List<T>;
                if(itemsList != null)
                {
                    var newKey = key.ToString();
                    return itemsList.FindIndex(item => 
                    { 
                        var value = prop.GetValue(item);
                        return value != null && value.ToString() == newKey;
                    });
                }
            }
            throw new NotSupportedException();
        }

        public T? Find(PropertyDescriptor prop, object key)
        {
            int index = FindCore(prop, key);
            return index != -1 ? this[index] : default;
        }




    }
}
