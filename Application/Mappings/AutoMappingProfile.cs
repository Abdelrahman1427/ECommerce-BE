using AutoMapper;

namespace Application.Mappings
{
    public static class AutoMappingProfile
    {
        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BranchMapping());
                cfg.AddProfile(new ProductMapping());
                cfg.AddProfile(new OrderMapping());
                cfg.AddProfile(new NotificationMapping());
                cfg.AddProfile(new CategoryMapping());
                cfg.AddProfile(new DiscountMapping());
            }).CreateMapper();
        }
    }
}
