﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace CS.CommonRc {
    public struct Range<T> where T:IComparable<T> {
        public T Lower;
        public T Upper;
        public T Offset;

        public Range(T upper, T lower, T offset = default(T)) {
            Lower = lower;
            Upper = upper;
            Offset = offset;
        }

        public static Range<T> Zero = new Range<T>(default(T), default(T), default(T));

        public T Difference { get { return Operator<T>.Subtract(Upper, Lower); } }
        public T UpperDifference { get { return Operator<T>.Subtract(Upper, Offset); } }
        public T LowerDifference { get { return Operator<T>.Subtract(Offset, Lower); } }
    }
}
