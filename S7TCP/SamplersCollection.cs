using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    public class SamplersCollection:IEnumerable<SamplerBase>
    {
        public List<SamplerBase> samplers = new List<SamplerBase>();

        public IEnumerator<SamplerBase> GetEnumerator()
        {
            return new SamplerEnumerator(samplers);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SamplerEnumerator(samplers);
        }

        public void Add(SamplerBase sampler)
        {
            samplers.Add(sampler);
        }

        public void Remove(SamplerBase sampler)
        {
            samplers.Remove(sampler);
            foreach(var variable in sampler)
            {
                variable.RemoveSampler();
            }
        }

        public SamplerBase this[int index]
        {
            get
            {
                return this.samplers[index];
            }

            set
            {
                this.samplers[index] = value;
            }
        }
    }

    public class SamplerEnumerator:IEnumerator<SamplerBase>
    {
        protected List<SamplerBase> samplers;

        protected int currentIndex = -1;

        public SamplerEnumerator(List<SamplerBase> samplersCollection)
        {
            samplers = samplersCollection;

        }

        public SamplerBase Current
        {
            get
            {
                if(this.currentIndex == -1)
                {
                    throw new InvalidOperationException("Use MoveNext before calling Current");
                }

                
                return samplers.ElementAt(currentIndex);
            }
        }

        public void Dispose()
        {
            
        }

        Object System.Collections.IEnumerator.Current
        {
             get
            {
                if(this.currentIndex == -1)
                {
                    throw new InvalidOperationException("Use MoveNext before calling Current");
                }

                
                return samplers.ElementAt(currentIndex);
            }
        }

        public bool MoveNext()
        {
            currentIndex++;

            if(currentIndex >= samplers.Count)
            {
                currentIndex = samplers.Count-1;
                return false;
            }

            return true;
        }

        public void Reset()
        {
            currentIndex = -1;
        }
    }
}
