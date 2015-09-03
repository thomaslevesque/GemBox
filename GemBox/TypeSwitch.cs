using System;

namespace GemBox
{
    public static class TypeSwitch
    {
        public static TypeSwitch<TBase> On<TBase>(TBase value) where TBase : class
        {
            return new TypeSwitch<TBase>(value);
        }

        public static TypeSwitchWithResultBuilder<TResult> WithResult<TResult>()
        {
            return new TypeSwitchWithResultBuilder<TResult>();
        }

        public class TypeSwitchWithResultBuilder<TResult>
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public TypeSwitch<TBase, TResult> On<TBase>(TBase value) where TBase : class
            {
                return new TypeSwitch<TBase, TResult>(value);
            }
        }

    }

    public class TypeSwitch<TBase> where TBase : class
    {
        private readonly TBase _value;
        public TypeSwitch(TBase value)
        {
            _value = value;
        }

        private bool _matched;

        public TypeSwitch<TBase> Case<T>(Action<T> action) where T : TBase
        {
            if (!_matched && _value is T)
            {
                _matched = true;
                action((T)_value);
            }
            return this;
        }

        public TypeSwitch<TBase> Default(Action<TBase> action)
        {
            if (!_matched)
            {
                _matched = true;
                action(_value);
            }
            return this;
        }
    }

    public class TypeSwitch<TBase, TResult> where TBase : class
    {
        private readonly TBase _value;
        public TypeSwitch(TBase value)
        {
            _value = value;
        }

        private bool _matched;
        private TResult _result;

        public TypeSwitch<TBase, TResult> Case<T>(Func<T, TResult> func) where T : TBase
        {
            if (!_matched && _value is T)
            {
                _matched = true;
                _result = func((T)_value);
            }
            return this;
        }

        public TypeSwitch<TBase, TResult> Default(Func<TBase, TResult> func)
        {
            if (!_matched)
            {
                _matched = true;
                _result = func(_value);
            }
            return this;
        }

        public TResult Result
        {
            get
            {
                if (!_matched)
                    throw new InvalidOperationException("No case matched");
                return _result;
            }
        }

        public static implicit operator TResult(TypeSwitch<TBase, TResult> typeSwitch)
        {
            return typeSwitch.Result;
        }
    }
}
