using LoanApplication.Controllers;
using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.Repositories;
using LoanApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LoanApplication.Tests
{
    
    public class Tests
    {
        [Fact]
        public void LoansListViewTest()
        {

            var loanRepositoryMock = new Mock<ILoanRepository>();
            var loanActionRepositoryMock = new Mock<ILoanActionRepository>();
            var UserRepositoryMock = new Mock<IUserRepository>();
            loanRepositoryMock.Setup(obj => obj.GetAll()).Returns(GetTestLoans());

            LoanController controller = new LoanController(
                loanRepositoryMock.Object,
                loanActionRepositoryMock.Object,
                UserRepositoryMock.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Loan>>(viewResult.Model);
            Assert.Equal(GetTestLoans().Count, model.Count());
        }

        private List<Loan> GetTestLoans()
        {
            var loans = new List<Loan>
            {
                new Loan { Id=1, Name="Party", Description="Party loans" },
                new Loan { Id=2, Name="Birthday", Description="Birthday loans" },
            };
            return loans;
        }

        [Fact]
        public void LoanActionsListViewTest()
        {

            var loanRepositoryMock = new Mock<ILoanRepository>();
            var loanActionRepositoryMock = new Mock<ILoanActionRepository>();
            var UserRepositoryMock = new Mock<IUserRepository>();
            int loanId = 1;
            loanActionRepositoryMock.Setup(obj => obj.Get(loanId)).Returns(GetTestLoanActions());

            LoanController controller = new LoanController(
                loanRepositoryMock.Object,
                loanActionRepositoryMock.Object,
                UserRepositoryMock.Object);

            // Act
            var result = controller.Show(loanId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<LoanView>(viewResult.Model);
            Assert.Equal(GetTestLoanActions().Count, model.Actions.Count());
        }

        private List<LoanAction> GetTestLoanActions()
        {
            Loan loan = new Loan { Id = 1, Name = "Party", Description = "Party loans" };
            var LoanActions = new List<LoanAction>
            {
                new LoanAction { Id=1, Loan=loan, Amount=500, Purpose="For buying a phone" },
                new LoanAction { Id=1, Loan=loan, Amount=-500, Purpose="Resolving loan for buying a phone" },
            };
            return LoanActions;
        }
        [Fact]
        public void AddLoanReturnsViewResultWithNullModel()
        {
            // Arrange
            var loanRepositoryMock = new Mock<ILoanRepository>();
            var loanActionRepositoryMock = new Mock<ILoanActionRepository>();
            var UserRepositoryMock = new Mock<IUserRepository>();

            LoanController controller = new LoanController(
                loanRepositoryMock.Object,
                loanActionRepositoryMock.Object,
                UserRepositoryMock.Object);

            controller.ModelState.AddModelError("Name", "Required");
            AddLoanModel addLoanModel = new AddLoanModel();

            // Act
            var result = controller.Create(addLoanModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(null, viewResult?.Model);
        }
        [Fact]
        public void AddLoanReturnsARedirectAndAddsLoan()
        {
            // Arrange
            var loanRepositoryMock = new Mock<ILoanRepository>();
            var loanActionRepositoryMock = new Mock<ILoanActionRepository>();
            var UserRepositoryMock = new Mock<IUserRepository>();

            LoanController controller = new LoanController(
                loanRepositoryMock.Object,
                loanActionRepositoryMock.Object,
                UserRepositoryMock.Object);

            AddLoanModel addLoanModel = new AddLoanModel();
            addLoanModel.Name = "Party";
            addLoanModel.Description = "Party on 9th November";

            // Act
            var result = controller.Create(addLoanModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Loan", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}