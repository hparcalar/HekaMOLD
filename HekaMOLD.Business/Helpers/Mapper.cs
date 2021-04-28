using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Helpers
{
    public static class Mapper
    {
        public static V MapTo<T, V>(this T from, V to)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<T, V>();
            });

            IMapper iMapper = config.CreateMapper();
            iMapper.Map<T, V>(from, to);

            return to;
        }

        public static int ToInt32(this string text)
        {
            return Convert.ToInt32(text);
        }
    }
}
