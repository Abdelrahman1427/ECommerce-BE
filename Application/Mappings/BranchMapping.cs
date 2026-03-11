using Application.Common.BranchDTOS;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class BranchMapping : Profile
{
    public BranchMapping()
    {
        CreateMap<AddBranchDTO, Branch>().ReverseMap();

        CreateMap<UpdateBranchDTO, Branch>().ReverseMap();

        CreateMap<Branch, GetBranchDTO>().ReverseMap();
        CreateMap<BranchLookUp, Branch>().ReverseMap();


    }
}
