using System;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter7
{
    class ZipCode
    {
        string _state;
        string _code;
        string _extension;

        public ZipCode(string state, string code, string extension)
        {
            _state = state;
            _code = code;
            _extension = extension;
        }

        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }

        public override string ToString()
        {
            return _state + _code + "-" + _extension + " (ZipCode)";
        }
    }
}
