using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Tests.Datasets;
using ShoppingCart.Utility;
using ShoppingCart.Web.Areas.Admin.Controllers;
using System.Linq.Expressions;
using Xunit;


namespace ShoppingCart.Tests
{
    public class CategoryControllerTests
    {
        [Fact]
        public void GetCategories_All_ReturnAllCategories()
        {
            // Arrange
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();

            repositoryMock.Setup(r => r.GetAll(It.IsAny<string>()))
                .Returns(() => CategoryDataset.Categories);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.Equal(CategoryDataset.Categories, result.Categories);
        }
        // Test data
        public static IEnumerable<object[]> CategoryData => new List<object[]>
        {
            new object[] { 0, "New Category" },        // Creating a new category
            new object[] { 1, "Updated Category" }     // Updating an existing category
        };

        [Theory]
        [MemberData(nameof(CategoryData))]
        public void CreateUpdate_CreatesOrUpdatesCategory(int categoryId, string categoryName)
        {
            // Arrange
            var category = new Category { Id = categoryId, Name = categoryName };
            var categoryVM = new CategoryVM { Category = category };

            var repositoryMock = new Mock<ICategoryRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Category).Returns(repositoryMock.Object);

            var controller = new CategoryController(unitOfWorkMock.Object);

            // Act
            controller.CreateUpdate(categoryVM);

            // Assert
            if (categoryId == 0)
            {
                repositoryMock.Verify(r => r.Add(category), Times.Once);  // Verify adding new category
            }
            else
            {
                repositoryMock.Verify(r => r.Update(category), Times.Once);  // Verify updating existing category
            }

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);  // Verify that Save method was called
        }
      
       

        [Fact]
        public void Get_ReturnsAllCategories()
        {
            // Arrange
            var expectedCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var repositoryMock = new Mock<ICategoryRepository>();
            repositoryMock.Setup(r => r.GetAll(null)).Returns(expectedCategories);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Category).Returns(repositoryMock.Object);

            var controller = new CategoryController(unitOfWorkMock.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.Equal(expectedCategories, result.Categories);
        }

        [Fact]
        public void OrderDetails_ReturnsCorrectViewModel()
        {
            // Arrange
            var orderHeaderId = 1;
            var orderHeader = new OrderHeader { Id = orderHeaderId, ApplicationUser = new ApplicationUser() };
            var orderDetails = new List<OrderDetail>
        {
            new OrderDetail { OrderHeaderId = orderHeaderId, Product = new Product() }
        };

            var mockOrderHeaderRepository = new Mock<IOrderHeaderRepository>();
            mockOrderHeaderRepository.Setup(r => r.GetT(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderHeader, bool>>>(), "ApplicationUser"))
                                     .Returns(orderHeader);

            var mockOrderDetailRepository = new Mock<IOrderDetailRepository>();
            mockOrderDetailRepository.Setup(r => r.GetAll("Product"))
                                     .Returns(orderDetails.AsQueryable());

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.OrderHeader).Returns(mockOrderHeaderRepository.Object);
            mockUnitOfWork.Setup(uow => uow.OrderDetail).Returns(mockOrderDetailRepository.Object);

            var controller = new OrderController(mockUnitOfWork.Object);

            // Act
            var result = controller.OrderDetails(orderHeaderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderHeader, result.OrderHeader);
            Assert.Equal(orderDetails, result.OrderDetails.ToList());
        }


    }
}

    
