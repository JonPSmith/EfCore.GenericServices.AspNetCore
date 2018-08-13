

using AutoMapper;

namespace Test.Helpers
{
    public static class BizRunnerHelpers
    {

        public static IMapper CreateEmptyMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                
            });
            var mapper = config.CreateMapper();
            return mapper;
        }

        public static IMapper CreateMapper<T>() where T : Profile, new()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new T());
            });
            var mapper = config.CreateMapper();
            return mapper;
        }
    }
}