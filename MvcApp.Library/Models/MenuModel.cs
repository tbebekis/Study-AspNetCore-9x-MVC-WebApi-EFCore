namespace MvcApp.Models
{
    /// <summary>
    /// Represents a menu item.
    /// <para>Used by the menu component.</para>
    /// </summary>
    public class MenuModel: IList
    {
        protected List<MenuModel> list = new List<MenuModel>();
        protected MenuModel fParent = null;

        #region explicit IList implementation
        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly => false;
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => (list as ICollection).SyncRoot;

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        void ICollection.CopyTo(Array Array, int Index) => (list as IList).CopyTo(Array, Index);
        int IList.Add(object Value)
        {
            if (Value is MenuModel)
                Add(Value as MenuModel);
            return list.Count;
        }
        bool IList.Contains(object Value)
        {
            if (Value is MenuModel)
                return Contains(Value as MenuModel);
            return false;
        }
        int IList.IndexOf(object Value)
        {
            if (Value is MenuModel)
                return IndexOf(Value as MenuModel);
            return -1;
        }
        void IList.Insert(int Index, object Value)
        {
            if (Value is MenuModel)
                Insert(Index, Value as MenuModel);
        }
        void IList.Remove(object Value)
        {
            if (Value is MenuModel)
                Remove(Value as MenuModel);
        }
        object IList.this[int Index]
        {
            get
            {
                return list[Index] as object;
            }
            set
            {
                if (value is MenuModel)
                    list[Index] = value as MenuModel;
            }
        }
        #endregion

        public MenuModel()
        {
        }

        /// <summary>
        /// Initializes all members of this instance
        /// </summary>
        public virtual void Clear()
        {
            if (list.Count > 0)
            {
                list.Clear();
            }
        }

        /// <summary>
        /// Adds a MenuItem
        /// </summary>
        public MenuModel Add()
        {
            MenuModel Result = (MenuModel)this.GetType().Create();
            Add(Result);
            return Result;
        }
        public MenuModel Add(string Text, string Url = null, string IconClass = null)
        {
            MenuModel Result = Add();
            Result.Text = Text;
            Result.Url = Url;
            Result.IconClass = IconClass;
            return Result;
        }
        /// <summary>
        /// Adds MenuItem and returns the current number of items in the list
        /// </summary>
        public virtual int Add(MenuModel MenuItem)
        {
            Insert(list.Count, MenuItem);
            return list.Count;
        }
        /// <summary>
        /// Inserts a MenuItem at Index
        /// </summary>
        public MenuModel Insert(int Index)
        {
            MenuModel Result = (MenuModel)this.GetType().Create();
            Insert(Index, Result);
            return Result;
        }
        /// <summary>
        /// Inserts MenuItem at Index
        /// </summary>
        public virtual void Insert(int Index, MenuModel MenuItem)
        {
            if (list.Contains(MenuItem))
                throw new ApplicationException("Can not insert MenuItem. MenuItem already contained in list.");

 
            list.Insert(Index, MenuItem);
            MenuItem.fParent = this; 
        }
        /// <summary>
        /// Removes the node at the Index
        /// </summary>
        public void RemoveAt(int Index)
        {
            list.RemoveAt(Index);
        }
        /// <summary>
        /// Removes the MenuItem
        /// </summary>
        public void Remove(MenuModel MenuItem)
        {
            list.Remove(MenuItem);
        }
        /// <summary>
        /// Returns true if the MenuItem is in the list
        /// </summary>
        public bool Contains(MenuModel MenuItem)
        {
            return list.Contains(MenuItem);
        }
        /// <summary>
        /// Returns true if the MenuItem contained in the tree that
        /// has this instance as parent.
        /// </summary>
        public bool TreeContains(MenuModel MenuItem)
        {
            if (list.Contains(MenuItem))
                return true;

            foreach (var child in list)
                if (child.TreeContains(MenuItem))
                    return true;

            return false;
        }
        /// <summary>
        /// Returns the index of the MenuItem in the list.
        /// </summary>
        public int IndexOf(MenuModel MenuItem)
        {
            return list.IndexOf(MenuItem);
        }

        /// <summary>
        /// Returns the first child, if any, else null.
        /// </summary>
        public MenuModel FirstChild()
        {
            if (list.Count > 0)
                return list[0];
            return null;
        }
        /// <summary>
        /// Returns the next child after MenuItem, if any, else null.
        /// </summary>
        public MenuModel NextChild(MenuModel MenuItem)
        {
            int Index = list.IndexOf(MenuItem);

            if ((Index != -1) && ((Index + 1 >= 0) && (Index + 1 <= list.Count - 1)))
                return list[Index + 1];
            return null;
        }
        /// <summary>
        /// Returns the previous child before MenuItem, if any, else null.
        /// </summary>
        public MenuModel PrevChild(MenuModel MenuItem)
        {
            int Index = list.IndexOf(MenuItem);

            if ((Index != -1) && ((Index - 1 >= 0) && (Index - 1 <= list.Count - 1)))
                return list[Index - 1];
            return null;
        }
        /// <summary>
        /// Returns the last child, if any, else null.
        /// </summary>
        public MenuModel LastChild()
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            return null;
        }
        /// <summary>
        /// Returns the next sibling MenuItem. If Parent is null, null is returned.
        /// </summary>
        /// <returns></returns>
        public MenuModel NextSibling()
        {
            if (fParent != null)
                return fParent.NextChild(this);
            return null;
        }
        /// <summary>
        /// Returns the previous sibling MenuItem. If Parent is null, null is returned.
        /// </summary>
        public MenuModel PrevSibling()
        {
            if (fParent != null)
                return fParent.PrevChild(this);
            return null;
        }
 
        // ● properties
        public string Id { get; set; }
        public string Text { get; set; }
        public string Content { get; set; }
        public string IconClass { get; set; }
        public string Url { get; set; }
        public bool IsSeparator { get; set; }
        public object Tag { get; set; }

        /// <summary>
        /// Returns the number of elements in the collection.
        /// </summary>
        [JsonIgnore]
        public int Count => list.Count;
 
        /// <summary>
        /// Indexer
        /// </summary>
        [JsonIgnore]
        public MenuModel this[int Index] { get { return list[Index]; } }
        /// <summary>
        /// Returns the children of this instance.
        /// </summary>
        [JsonIgnore]
        public MenuModel[] Children => list.ToArray();
        /// <summary>
        /// Returns the root of the tree this MenuItem may belong to.
        /// </summary>
        [JsonIgnore]
        public MenuModel Root
        {
            get
            {
                MenuModel Current = this;
                MenuModel CurrentParent = fParent;

                while (CurrentParent != null)
                {
                    Current = CurrentParent;
                    CurrentParent = Current.fParent;
                }

                return Current;
            }
        }
        /// <summary>
        /// Returns true if this MenuItem is the root in a tree.
        /// </summary>
        public bool IsRoot => Root == this || Root == null;
        /// <summary>
        /// Returns the parent of this MenuItem, if any, else null.
        /// </summary>
        [JsonIgnore]
        public MenuModel Parent => fParent;
        /// <summary>
        /// Returns true if this MenuItem has child items.
        /// </summary>
        [JsonIgnore]
        public bool HasChildNodes => list.Count > 0;
        /// <summary>
        /// Returns the level of this MenuItem. A root node has level 0, its children have level 1 and so on.
        /// </summary>
        [JsonIgnore]
        public int Level => fParent == null ? 0 : fParent.Level + 1;
        /// <summary>
        /// Returns the index of this MenuItem in the list of its parent, if any, else -1
        /// </summary>
        [JsonIgnore]
        public int Index => fParent == null ? -1 : fParent.list.IndexOf(this);
        /// <summary>
        /// Returns the total number of items of this node and its child items.
        /// </summary>
        [JsonIgnore]
        public int TotalCount
        {
            get
            {
                int Result = list.Count;
                foreach (MenuModel Child in list)
                    Result += Child.TotalCount;
                return Result;
            }
        }

    }
}
