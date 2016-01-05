using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace Models
{
    public class LanguageResourcesList
    {
        public class Resource
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string Culture { get; set; }
        }

        private IResourceReader _reader = null;

        public string CultureName { get; set; }

        public LanguageResourcesList(IResourceReader reader)
        {
            _reader = reader;
        }

        private IList<Resource> _resources = null;
        public IList<Resource> Resources
        {
            get
            {
                if (_resources == null)
                {
                    _resources = new List<Resource>();
                    foreach (DictionaryEntry item in _reader)
                    {
                        _resources.Add(new Resource { Key = item.Key.ToString(), Value = item.Value.ToString() });
                    }
                }
                return _resources;
            }
        }
    }
}
