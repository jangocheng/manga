﻿namespace Manga.Application.UseCases.CloseAccount
{
    using System.Threading.Tasks;
    using Manga.Domain.Customers;
    using Manga.Domain.Customers.Accounts;

    public class CloseInteractor : IInputBoundary<CloseCommand>
    {
        private readonly ICustomerReadOnlyRepository customerReadOnlyRepository;
        private readonly ICustomerWriteOnlyRepository customerWriteOnlyRepository;
        private readonly IOutputBoundary<CloseResponse> outputBoundary;
        private readonly IResponseConverter responseConverter;

        public CloseInteractor(
            ICustomerReadOnlyRepository customerReadOnlyRepository,
            ICustomerWriteOnlyRepository customerWriteOnlyRepository,
            IOutputBoundary<CloseResponse> outputBoundary,
            IResponseConverter responseConverter)
        {
            this.customerReadOnlyRepository = customerReadOnlyRepository;
            this.customerWriteOnlyRepository = customerWriteOnlyRepository;
            this.outputBoundary = outputBoundary;
            this.responseConverter = responseConverter;
        }

        public async Task Handle(CloseCommand request)
        {
            Customer customer = await customerReadOnlyRepository.GetByAccount(request.AccountId);
            Account account = customer.FindAccount(request.AccountId);

            customer.RemoveAccount(request.AccountId);
            await customerWriteOnlyRepository.Update(customer);

            CloseResponse response = responseConverter.Map<CloseResponse>(account);
            this.outputBoundary.Populate(response);
        }
    }
}