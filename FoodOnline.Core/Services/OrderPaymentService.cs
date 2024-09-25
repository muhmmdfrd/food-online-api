using AutoMapper;
using Flozacode.Models.Paginations;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Interfaces;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;

namespace FoodOnline.Core.Services;

public class OrderPaymentService : IOrderPaymentService
{
    private readonly IFlozaRepo<OrderPayment, AppDbContext> _repo;
    private readonly IMapper _mapper;

    public OrderPaymentService(IFlozaRepo<OrderPayment, AppDbContext> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public Task<Pagination<OrderPaymentViewDto>> GetPagedAsync(OrderPaymentFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<List<OrderPaymentViewDto>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OrderPaymentViewDto> FindAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateAsync(OrderPaymentAddDto value)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(OrderPaymentUpdDto value)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }
}