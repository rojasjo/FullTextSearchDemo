using FullTextSearchDemo.SearchEngine.Tests.TestModels;

public static class VehicleHelper
{
    public static List<Vehicle> GenerateFixedVehicles()
    {
        return new List<Vehicle>
        {
            new() { Id = 1, Name = "Car1", Type = "Car", Brand = "Toyota", Model = "Model1", Year = 2020 },
            new() { Id = 2, Name = "Car2", Type = "Car", Brand = "Honda", Model = "Model2", Year = 2021 },
            new() { Id = 3, Name = "Car3", Type = "Car", Brand = "Ford", Model = "Model3", Year = 2022 },

            new()
            {
                Id = 4, Name = "Motorcycle1", Type = "Motorcycle", Brand = "Harley-Davidson", Model = "Model4",
                Year = 2020
            },
            new()
            {
                Id = 5, Name = "Motorcycle2", Type = "Motorcycle", Brand = "Yamaha", Model = "Model5", Year = 2021
            },

            new() { Id = 6, Name = "Truck1", Type = "Truck", Brand = "Chevrolet", Model = "Model6", Year = 2022 },
            new() { Id = 7, Name = "Truck2", Type = "Truck", Brand = "BMW", Model = "Model7", Year = 2023 },


            new() { Id = 8, Name = "Car4", Type = "Car", Brand = "Toyota", Model = "Model8", Year = 2023 },
            new() { Id = 9, Name = "Car5", Type = "Car", Brand = "Honda", Model = "Model9", Year = 2022 },
            new() { Id = 10, Name = "Car6", Type = "Car", Brand = "Ford", Model = "Model10", Year = 2021 },

            new()
            {
                Id = 11, Name = "Motorcycle3", Type = "Motorcycle", Brand = "Harley-Davidson", Model = "Model11",
                Year = 2023
            },
            new()
            {
                Id = 12, Name = "Motorcycle4", Type = "Motorcycle", Brand = "Yamaha", Model = "Model12", Year = 2022
            },

            new()
            {
                Id = 13, Name = "Truck3", Type = "Truck", Brand = "Chevrolet", Model = "Model13", Year = 2021
            },
            new() { Id = 14, Name = "Truck4", Type = "Truck", Brand = "BMW", Model = "Model14", Year = 2020 },


            new() { Id = 15, Name = "Car7", Type = "Car", Brand = "Toyota", Model = "Model15", Year = 2022 },
            new() { Id = 16, Name = "Car8", Type = "Car", Brand = "Honda", Model = "Model16", Year = 2021 },
            new() { Id = 17, Name = "Car9", Type = "Car", Brand = "Ford", Model = "Model17", Year = 2020 },

            new()
            {
                Id = 18, Name = "Motorcycle5", Type = "Motorcycle", Brand = "Harley-Davidson", Model = "Model18",
                Year = 2021
            },
            new()
            {
                Id = 19, Name = "Motorcycle6", Type = "Motorcycle", Brand = "Yamaha", Model = "Model19", Year = 2022
            },
            new()
            {
                Id = 20, Name = "Truck5", Type = "Truck", Brand = "Chevrolet", Model = "Model20", Year = 2023
            },
            new() { Id = 21, Name = "Truck6", Type = "Truck", Brand = "BMW", Model = "Model21", Year = 2022 },
            new() { Id = 22, Name = "Car10", Type = "Car", Brand = "Toyota", Model = "Model22", Year = 2023 },
            new()
            {
                Id = 23, Name = "Motorcycle7", Type = "Motorcycle", Brand = "Harley-Davidson", Model = "Model23",
                Year = 2020
            },
            new()
            {
                Id = 24, Name = "Truck7", Type = "Truck", Brand = "Chevrolet", Model = "Model24", Year = 2021
            },
            new() { Id = 25, Name = "Car11", Type = "Car", Brand = "Honda", Model = "Model25", Year = 2022 }
        };
    }
}