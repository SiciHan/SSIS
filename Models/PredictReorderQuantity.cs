//@SHutong
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.Models
{
    public class PredictReorderQuantity
    {
        public double Mean(List<int> datas)
        {
            double mean = 0;
            double sum = 0;
            foreach (int d in datas)
            {
                sum += d;
            }
            mean = sum / (double)datas.Count;
            return mean;
        }

        public double Std(List<int> datas)
        {
            double mean = Mean(datas);
            double s = 0;
            foreach (int d in datas)
            {
                s += Math.Pow((mean - d), 2);
            }
            return Math.Sqrt(s / (double)datas.Count);
        }

        public double IntervalEstimate(List<int> datas)
        {
            double std = Std(datas);
            double interval = Mean(datas) + 1.96 * Std(datas) / Math.Sqrt(datas.Count);
            return interval;
        }
    }
}