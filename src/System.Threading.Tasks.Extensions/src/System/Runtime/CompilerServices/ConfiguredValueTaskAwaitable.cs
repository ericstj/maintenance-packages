// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

#if !NETSTANDARD2_0
using System.Runtime.CompilerServices;
#endif

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="ValueTask"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredValueTaskAwaitable
    {
        /// <summary>The wrapped <see cref="Task"/>.</summary>
        private readonly ValueTask _value;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="ValueTask"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ConfiguredValueTaskAwaitable(ValueTask value) => _value = value;

        /// <summary>Returns an awaiter for this <see cref="ConfiguredValueTaskAwaitable"/> instance.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConfiguredValueTaskAwaiter GetAwaiter() => new ConfiguredValueTaskAwaiter(_value);

        /// <summary>Provides an awaiter for a <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>The value being awaited.</summary>
            private readonly ValueTask _value;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ConfiguredValueTaskAwaiter(ValueTask value) => _value = value;

            /// <summary>Gets whether the <see cref="ConfiguredValueTaskAwaitable"/> has completed.</summary>
            public bool IsCompleted
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _value.IsCompleted;
            }

            /// <summary>Gets the result of the ValueTask.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [StackTraceHidden]
            public void GetResult() => _value.ThrowIfCompletedUnsuccessfully();

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
            public void OnCompleted(Action continuation)
            {
                object obj = _value._obj;
                Debug.Assert(obj == null || obj is Task || obj is IValueTaskSource);

                if (obj is Task t)
                {
                    t.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
                else if (obj != null)
                {
                    Unsafe.As<IValueTaskSource>(obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        ValueTaskSourceOnCompletedFlags.FlowExecutionContext |
                            (_value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None));
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
            }

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
            public void UnsafeOnCompleted(Action continuation)
            {
                object obj = _value._obj;
                Debug.Assert(obj == null || obj is Task || obj is IValueTaskSource);

                if (obj is Task t)
                {
                    t.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
                else if (obj != null)
                {
                    Unsafe.As<IValueTaskSource>(obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        _value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
            }
        }
    }

    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="ValueTask{TResult}"/>.</summary>
    /// <typeparam name="TResult">The type of the result produced.</typeparam>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredValueTaskAwaitable<TResult>
    {
        /// <summary>The wrapped <see cref="ValueTask{TResult}"/>.</summary>
        private readonly ValueTask<TResult> _value;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="ValueTask{TResult}"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ConfiguredValueTaskAwaitable(ValueTask<TResult> value) => _value = value;

        /// <summary>Returns an awaiter for this <see cref="ConfiguredValueTaskAwaitable{TResult}"/> instance.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConfiguredValueTaskAwaiter GetAwaiter() => new ConfiguredValueTaskAwaiter(_value);

        /// <summary>Provides an awaiter for a <see cref="ConfiguredValueTaskAwaitable{TResult}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>The value being awaited.</summary>
            private readonly ValueTask<TResult> _value;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ConfiguredValueTaskAwaiter(ValueTask<TResult> value) => _value = value;

            /// <summary>Gets whether the <see cref="ConfiguredValueTaskAwaitable{TResult}"/> has completed.</summary>
            public bool IsCompleted
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _value.IsCompleted;
            }

            /// <summary>Gets the result of the ValueTask.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [StackTraceHidden]
            public TResult GetResult() => _value.Result;

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable{TResult}"/>.</summary>
            public void OnCompleted(Action continuation)
            {
                object obj = _value._obj;
                Debug.Assert(obj == null || obj is Task<TResult> || obj is IValueTaskSource<TResult>);

                if (obj is Task<TResult> t)
                {
                    t.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
                else if (obj != null)
                {
                    Unsafe.As<IValueTaskSource<TResult>>(obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        ValueTaskSourceOnCompletedFlags.FlowExecutionContext |
                            (_value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None));
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                }
            }

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable{TResult}"/>.</summary>
            public void UnsafeOnCompleted(Action continuation)
            {
                object obj = _value._obj;
                Debug.Assert(obj == null || obj is Task<TResult> || obj is IValueTaskSource<TResult>);

                if (obj is Task<TResult> t)
                {
                    t.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
                else if (obj != null)
                {
                    Unsafe.As<IValueTaskSource<TResult>>(obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token,
                        _value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
                }
                else
                {
                    ValueTask.CompletedTask.ConfigureAwait(_value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                }
            }
        }
    }
}
