﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Pocket.Common.Flows
{
    public class BufferedFlow<T> : IFlow<IEnumerable<T>>
    {
        private readonly IFlux<IEnumerable<T>> _flux;
        
        public BufferedFlow(IFlow<T> source, IVoidFlow bufferFlow)
        {
            _flux = new PureFlux<IEnumerable<T>>(Enumerable.Empty<T>());

            var dog = new object();
            var buffer = new List<T>();
            
            source.OnNext(x =>
            {
                lock (dog) buffer.Add(x);
            });
            
            bufferFlow.OnNext(() =>
            {
                lock (dog)
                {
                    _flux.Pulse(buffer);

                    buffer = new List<T>();    
                }
            });
        }

        public IEnumerable<T> Current =>
            _flux.Current;
        public IDisposable OnNext(Action<IEnumerable<T>> action) =>
            _flux.OnNext(action);
    }
}