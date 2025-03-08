// using Microsoft.Extensions.Options;

// namespace SilvermineNordic.Repository.Services
// {
//     public class SilvermineNordicDbContextFactory : ISilvermineNordicDbContextFactory
//     {
//         private readonly IOptionsSnapshot<SilvermineNordicConfigurationService> _options;

//         public SilvermineNordicDbContextFactory(
//             IOptionsSnapshot<SilvermineNordicConfigurationService> options)
//         {
//             _options = options;
//         }

//         public SilvermineNordicDbContext Create()
//         {
//             return new SilvermineNordicDbContext(_options);
//         }
//     }
// }