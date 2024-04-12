using System.Linq;
using System.Text.RegularExpressions;

namespace WebApplicationTest
{
    internal class Car
    {
        public int Id { set; get; }

        public float MaxSpeed { set; get; }
        public string Model { set; get; }
        public string Description { set; get; }

        public Car()
        {
            Id = 0;

            MaxSpeed = 0.0F;
            Model = string.Empty;
            Description = string.Empty;
        }
    }

    public class Program
    {
        private static int lastCarId = 0;
        public static void Main(string[] args)
        {
            List<Car> listOfCars = new List<Car>();

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.Run(async (context) =>
            {
                var request = context.Request;
                var response = context.Response;
                var path = context.Request.Path;
                string expressionForNumber = "^/api/cars/([0-9]+)$";

                if (path == "/api/cars" && request.Method == "GET")
                {
                    await response.WriteAsJsonAsync(listOfCars);
                }
                else if (Regex.IsMatch(path, expressionForNumber) && request.Method == "GET")
                {
                    int intVariable = int.Parse(path.Value?.Split("/")[3]);

                    try
                    {
                        Car myCar = listOfCars.FirstOrDefault((inCar) => inCar.Id == intVariable);

                        if (myCar is null)
                        {
                            response.StatusCode = 404;

                            await response.WriteAsJsonAsync(new
                            {
                                message = "Car is not found!"
                            });
                        }
                        else
                        {
                            await response.WriteAsJsonAsync(myCar);
                        }
                    }
                    catch (Exception exception)
                    {
                        response.StatusCode = 500;

                        await response.WriteAsJsonAsync(new
                        {
                            message = "Server error is occured!"
                        });
                    }
                }
                else if (path == "/api/cars" && request.Method == "POST")
                {
                    try
                    {
                        Car tempCar = await request.ReadFromJsonAsync<Car>();

                        if (tempCar != null)
                        {
                            tempCar.Id = ++lastCarId;
                            listOfCars.Add(tempCar);

                            await response.WriteAsJsonAsync(tempCar);
                        }
                        else
                        {
                            response.StatusCode = 404;

                            await response.WriteAsJsonAsync(new
                            {
                                message = "Car is null!"
                            });
                        }
                    }
                    catch (Exception exception)
                    {
                        response.StatusCode = 500;
                        await response.WriteAsJsonAsync(new
                        {
                            message = "Server error is occured!"
                        });
                    }
                }
                else if (Regex.IsMatch(path, expressionForNumber) && request.Method == "PUT")
                {
                    int intVariable = int.Parse(path.Value?.Split("/")[3]);

                    try
                    {
                        Car carToUpdate = listOfCars.FirstOrDefault(inCar => inCar.Id == intVariable);
                        if (carToUpdate != null)
                        {
                            var updatedCar = await request.ReadFromJsonAsync<Car>();
                            carToUpdate.Model = updatedCar.Model;
                            carToUpdate.MaxSpeed = updatedCar.MaxSpeed;
                            carToUpdate.Description = updatedCar.Description;
                            await response.WriteAsJsonAsync(carToUpdate);
                        }
                        else
                        {
                            response.StatusCode = 404;
                            await response.WriteAsJsonAsync(new { message = "Car not found!" });
                        }
                    }
                    catch (Exception)
                    {
                        response.StatusCode = 500;
                        await response.WriteAsJsonAsync(new { message = "Server error occurred!" });
                    }
                }
                else if (Regex.IsMatch(path, expressionForNumber) && request.Method == "DELETE")
                {
                    int intVariable = int.Parse(path.Value?.Split("/")[3]);

                    try
                    {
                        Car carToDelete = listOfCars.FirstOrDefault(inCar => inCar.Id == intVariable);
                        if (carToDelete != null)
                        {
                            listOfCars.Remove(carToDelete);
                            response.StatusCode = 204; // No Content
                            await response.WriteAsync("Car deleted successfully");
                        }
                        else
                        {
                            response.StatusCode = 404;
                            await response.WriteAsJsonAsync(new { message = "Car not found!" });
                        }
                    }
                    catch (Exception)
                    {
                        response.StatusCode = 500;
                        await response.WriteAsJsonAsync(new { message = "Server error occurred!" });
                    }
                }
                else { response.ContentType = "text/html; charset=utf-8"; await response.SendFileAsync("html/index.html"); }
            });
            app.Run();
        }
    }
}