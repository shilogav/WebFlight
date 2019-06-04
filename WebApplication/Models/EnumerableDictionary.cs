using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class EnumerableDictionary<key, val> : IEnumerable<val>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<val> IEnumerable<val>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}