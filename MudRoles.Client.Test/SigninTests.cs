using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudRoles.Client.Infrastructure.Settings;
using MudRoles.Client.Pages.Generics;
using Xunit;
using Moq;
using RichardSzalay.MockHttp;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Components.Forms;

namespace MudRoles.Client.Test
{
    public class SigninTests : TestContext
    {
        private readonly Mock<ISnackbar> _snackbarMock;
        public SigninTests()
        {
            // Common setup for all tests
            Services.AddMudServices();
            Services.AddSingleton(new ThemeService());

            // Setup JSInterop to handle the calls
            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            JSInterop.SetupVoid("mudKeyInterceptor.disconnect", _ => true);
            JSInterop.SetupVoid("mudKeyInterceptor.updateOptions", _ => true);
            JSInterop.SetupVoid("watchDarkThemeMedia", _ => true);

            // Mock ISnackbar service
            _snackbarMock = new Mock<ISnackbar>();
            Services.AddSingleton(_snackbarMock.Object);
        }
        
        [Fact]
        public void RememberMe_Should_Be_Checked_When_User_Selects_It()
        {
            // Arrange
            var cut = RenderComponent<Signin>();

            // Act
            var rememberMeCheckbox = cut.FindComponent<MudCheckBox<bool>>();
            rememberMeCheckbox.Find("input").Change(true);

            // Assert
            Assert.True(rememberMeCheckbox.Instance.Value);
        }

        [Fact]
        public async Task Should_Clear_Error_Message_On_Input_Change()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/auth/signin")
                    .Respond(HttpStatusCode.Unauthorized);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton(httpClient);

            var cut = RenderComponent<Signin>();

            // Act
            await cut.Instance.LoginUser();
            var emailField = cut.FindComponent<MudTextField<string>>();
            emailField.Find("input").Change("new-email@example.com");

            // Allow time for the component to re-render
            await Task.Delay(500);

            // Debugging: Print the rendered HTML
            Console.WriteLine(cut.Markup);

            // Assert
            var validationSummary = cut.Find("div.error");
            Assert.DoesNotContain("Invalid login attempt.", validationSummary.TextContent);
        }
        

        [Fact]
        public void Should_Show_Validation_Message_For_Required_Email()
        {
            // Arrange
            var cut = RenderComponent<Signin>();

            // Act
            var emailField = cut.FindComponent<MudTextField<string>>();
            emailField.Find("input").Change(""); // Trigger "Email is required." validation
            emailField.Find("input").Blur(); // Trigger validation

            // Allow time for the component to re-render and validation to be displayed
            cut.WaitForState(() => cut.FindAll(".validation-message").Count > 0);

            // Debugging: Print the rendered HTML
            Console.WriteLine(cut.Markup);

            // Assert
            var validationMessages = cut.FindAll(".validation-message");
            Assert.Contains(validationMessages, message => message.TextContent.Contains("Email is required."));
        }

        [Fact]
        public void Should_Show_Validation_Message_For_Required_Email_And_Password()
        {
            // Arrange
            var cut = RenderComponent<Signin>();

            // Act
            var emailField = cut.FindComponent<MudTextField<string>>();
            var passwordField = cut.FindComponent<MudTextField<string>>();
            var loginButton = cut.FindComponent<MudButton>();

            // Set empty values to trigger required field validation
            emailField.Find("input").Change("");
            emailField.Find("input").Blur(); // Trigger validation

            passwordField.Find("input").Change("");
            passwordField.Find("input").Blur(); // Trigger validation

            // Trigger form submission
            loginButton.Find("button").Click();

            // Allow time for the component to re-render and validation to be displayed
            cut.WaitForState(() => cut.FindAll(".validation-message").Count > 0, TimeSpan.FromSeconds(10));

            // Debugging: Print the rendered HTML
            Console.WriteLine(cut.Markup);

            // Assert
            var validationMessages = cut.FindAll(".validation-message");
            foreach (var message in validationMessages)
            {
                Console.WriteLine(message.TextContent); // Print each validation message
            }
            Assert.Contains(validationMessages, message => message.TextContent.Contains("Email is required."));
            Assert.Contains(validationMessages, message => message.TextContent.Contains("Password is required."));
        }

        [Fact]
        public void LoginButton_Should_Be_Disabled_When_Fields_Are_Empty()
        {
            // Arrange
            var cut = RenderComponent<Signin>();

            // Act
            var loginButton = cut.FindComponent<MudButton>();

            // Assert
            Assert.True(loginButton.Instance.Disabled);
        }

        [Fact]
        public async Task FormSubmissionWithValidData()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/auth/signin")
                    .Respond(HttpStatusCode.OK);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton(httpClient);

            var cut = RenderComponent<Signin>();

            // Act
            var emailField = cut.FindComponent<MudTextField<string>>();
            var passwordField = cut.FindComponent<MudTextField<string>>();
            var loginButton = cut.FindComponent<MudButton>();

            emailField.Find("input").Change("test@example.com");
            passwordField.Find("input").Change("password");
            await cut.InvokeAsync(() => loginButton.Instance.OnClick.InvokeAsync());

            // Assert
            var navigationManagerProperty = cut.Instance.GetType().GetProperty("NavigationManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(navigationManagerProperty);
            var navigationManager = navigationManagerProperty?.GetValue(cut.Instance) as NavigationManager;
            Assert.NotNull(navigationManager);
            Assert.Equal("http://localhost/", navigationManager?.Uri);
        }

        [Fact]
        public async Task FormSubmissionWithInvalidData()
        {
            // Arrange
            var component = RenderComponent<Signin>();

            // Act
            component.Find("form").Submit();

            // Assert
            var validationSummary = component.Find("ul.validation-errors");
            Assert.Contains("Email is required.", validationSummary.TextContent);
            Assert.Contains("Password is required.", validationSummary.TextContent);
            await Task.CompletedTask;
        }

        [Fact]
        public void SigninRendersCorrectly()
        {
            // Arrange
            var cut = RenderComponent<Signin>();

            // Act
            var form = cut.Find("form");
            var textFields = cut.FindComponents<MudTextField<string>>();
            var emailField = textFields[0];
            var passwordField = textFields[1];
            var rememberMeCheckbox = cut.FindComponent<MudCheckBox<bool>>();
            var loginButton = cut.FindComponent<MudButton>();

            // Assert
            Assert.NotNull(form);
            Assert.NotNull(emailField);
            Assert.NotNull(passwordField);
            Assert.NotNull(rememberMeCheckbox);
            Assert.NotNull(loginButton);

            // Optionally, you can check specific properties
            Assert.Equal("name@example.com", emailField.Instance.Placeholder);
            Assert.Equal("password", passwordField.Instance.Placeholder);
        }


        [Fact]
        public async Task SuccessfulLoginRedirectsToHome()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/auth/signin")
                    .Respond(HttpStatusCode.OK);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton(httpClient);

            var cut = RenderComponent<Signin>();

            // Act
            await cut.Instance.LoginUser();

            // Assert
            var navigationManagerProperty = cut.Instance.GetType().GetProperty("NavigationManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(navigationManagerProperty);
            var navigationManager = navigationManagerProperty?.GetValue(cut.Instance) as NavigationManager;
            Assert.NotNull(navigationManager);
            Assert.Equal("http://localhost/", navigationManager?.Uri);
        }

        [Fact]
        public async Task InvalidLoginShowsErrorMessage()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/auth/signin")
                    .Respond(HttpStatusCode.Unauthorized);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton(httpClient);

            var cut = RenderComponent<Signin>();

            // Act
            await cut.Instance.LoginUser();

            // Assert
            var errorMessageField = cut.Instance.GetType().GetField("errorMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(errorMessageField);
            var errorMessage = errorMessageField?.GetValue(cut.Instance) as string;
            Assert.Equal("Error: Invalid login attempt.", errorMessage);
        }

        [Fact]
        public async Task UnexpectedErrorShowsErrorMessage()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/auth/signin")
                    .Respond(HttpStatusCode.InternalServerError);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton(httpClient);

            var cut = RenderComponent<Signin>();

            // Act
            await cut.Instance.LoginUser();

            // Assert
            var errorMessageField = cut.Instance.GetType().GetField("errorMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(errorMessageField);
            var errorMessage = errorMessageField?.GetValue(cut.Instance) as string;
            Assert.StartsWith("Error: An unexpected error occurred.", errorMessage);
        }
    }
}
