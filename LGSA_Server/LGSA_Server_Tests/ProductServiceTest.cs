using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LGSA_Server.Model;
using System.Collections.Generic;
using LGSA.Model.UnitOfWork;
using System.Data.Entity;
using LGSA.Model.Services;
using System.Threading.Tasks;
using LGSA.Model.Repositories;
using LGSA_Server.Model.Enums;
using System.Linq;
using System.Linq.Expressions;

namespace LGSA_Server_Tests
{
    [TestClass]
    public class ProductServiceTest
    {

        private Mock<DbUnitOfWork> PrepareFactoryForAdd(out Mock<IUnitOfWorkFactory> factory)
        {
            var data = new List<product>();
            var mockSet = new Mock<DbSet<product>>();
            mockSet.Setup(m => m.Add(It.IsAny<product>())).Returns((product prod) => { data.Add(prod); return prod; });

            var mockContext = new Mock<MainDatabaseEntities>();
            mockContext.Setup(m => m.product).Returns(mockSet.Object);

            var mockRepository = new Mock<ProductRepository>(mockContext.Object);
            mockRepository.Setup(m => m.Add(It.IsAny<product>())).Returns((product prod) => { return mockContext.Object.product.Add(prod); });
            mockRepository.Setup(m => m.GetData(It.IsAny<Expression<Func<product, bool>>>())).Returns(
                (
                    async (Expression<Func<product, bool>> expression) =>
                    {
                        var filter = expression.Compile();
                        var result = data.Where(filter);
                        return await Task.FromResult(result);
                    }
                ));

            var mockUnitOfWork = new Mock<DbUnitOfWork>();
            mockUnitOfWork.Setup(m => m.Context).Returns(mockContext.Object);
            mockUnitOfWork.Setup(m => m.ProductRepository).Returns(mockRepository.Object);

            factory = new Mock<IUnitOfWorkFactory>();
            factory.Setup(m => m.CreateUnitOfWork()).Returns(mockUnitOfWork.Object);

            return mockUnitOfWork;
        }
        private Mock<DbUnitOfWork> PrepareFactoryForUpdate(out Mock<IUnitOfWorkFactory> factory, bool shouldFail)
        {
            var data = new List<product>();
            data.Add(new product()
            {
                ID = 1,
                Name = "zz",
                stock = 7
            });
            var mockSet = new Mock<DbSet<product>>();
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns(
                (
                    async (object[] id) =>
                    {
                        var result = data.SingleOrDefault(p => p.ID == (int)id[0]);
                        return await Task.FromResult(result);
                    }
                ));
            var offerData = new List<sell_Offer>();
            if(shouldFail)
            {
                offerData.Add(new sell_Offer()
                {
                    ID = 1,
                    amount = 8,
                    name = "offer",
                    product_id = 1,
                    seller_id = 1,
                    Update_Date = DateTime.Now,
                    Update_Who = 1
                });
            }
            else
            {
                offerData.Add(new sell_Offer()
                {
                    ID = 1,
                    amount = 7,
                    name = "offer",
                    product_id = 1,
                    seller_id = 1,
                    Update_Date = DateTime.Now,
                    Update_Who = 1
                });
            }

            var mockContext = new Mock<MainDatabaseEntities>();
            mockContext.Setup(m => m.product).Returns(mockSet.Object);
            var mockRepository = new Mock<ProductRepository>(mockContext.Object);
            mockRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns(
                (
                    async (int id) => 
                    {
                        return await mockContext.Object.product.FindAsync(id);
                    }
                ));

            var mockOfferRepository = new Mock<SellOfferRepository>(mockContext.Object);
            mockOfferRepository.Setup(m => m.GetData(It.IsAny<Expression<Func<sell_Offer, bool>>>())).Returns(
                (
                    async (Expression<Func<sell_Offer, bool>> expression) =>
                    {
                        var filter = expression.Compile();
                        var result = offerData.Where(filter);
                        return await Task.FromResult(result);
                    }
                ));

            var mockUnitOfWork = new Mock<DbUnitOfWork>();
            mockUnitOfWork.Setup(m => m.Context).Returns(mockContext.Object);
            mockUnitOfWork.Setup(m => m.ProductRepository).Returns(mockRepository.Object);
            mockUnitOfWork.Setup(m => m.SellOfferRepository).Returns(mockOfferRepository.Object);

            factory = new Mock<IUnitOfWorkFactory>();
            factory.Setup(m => m.CreateUnitOfWork()).Returns(mockUnitOfWork.Object);

            return mockUnitOfWork;
        }
        [TestMethod]
        public async Task AddProduct()
        {
            Mock<IUnitOfWorkFactory> mockFactory = null;
            var unitOfWork = PrepareFactoryForAdd(out mockFactory);

            var service = new ProductService(mockFactory.Object);
            var p = new product()
            {
                ID = 0,
                product_owner = 1,
                Name = "product",
                Update_Date = DateTime.Now,
                Update_Who = 1,
                sold_copies = 0,
                stock = 1
            };
            var result = await service.Add(p);

            unitOfWork.Verify(m => m.Save(), Times.Once);
            Assert.IsTrue(result == ErrorValue.NoError);
        }

        [TestMethod]
        public async Task AddSameProduct()
        {
            Mock<IUnitOfWorkFactory> mockFactory = null;
            var unitOfWork = PrepareFactoryForAdd(out mockFactory);

            var service = new ProductService(mockFactory.Object);
            var product = new product()
            {
                ID = 0,
                product_owner = 1,
                Name = "product",
                Update_Date = DateTime.Now,
                Update_Who = 1,
                sold_copies = 0,
                stock = 1
            };
            var firstInsert = await service.Add(product);
            var secondInsert = await service.Add(product);

            unitOfWork.Verify(m => m.Save(), Times.Once);
            Assert.IsTrue(firstInsert == ErrorValue.NoError);
            Assert.IsTrue(secondInsert == ErrorValue.EntityExists);
        }
        [TestMethod]
        public async Task FailUpdateOfferedProduct()
        {
            Mock<IUnitOfWorkFactory> mockFactory = null;
            var unitOfWork = PrepareFactoryForUpdate(out mockFactory, true);
            var service = new ProductService(mockFactory.Object);

            var product = new product()
            {
                ID = 1,
                Name = "zz",
                stock = 7
            };

            var update = await service.Update(product);

            unitOfWork.Verify(m => m.Save(), Times.Never);
            Assert.IsTrue(update == ErrorValue.AmountGreaterThanStock);
        }
        [TestMethod]
        public async Task SucceedUpdateOfferedProduct()
        {
            Mock<IUnitOfWorkFactory> mockFactory = null;
            var unitOfWork = PrepareFactoryForUpdate(out mockFactory, false);
            var service = new ProductService(mockFactory.Object);

            var product = new product()
            {
                ID = 1,
                Name = "zz",
                stock = 7
            };

            var update = await service.Update(product);

            unitOfWork.Verify(m => m.Save(), Times.Once);
            Assert.IsTrue(update == ErrorValue.NoError);
        }
    }
}
