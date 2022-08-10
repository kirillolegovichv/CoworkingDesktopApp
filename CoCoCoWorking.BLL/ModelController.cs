﻿using CoCoCoWorking.DAL;
using CoCoCoWorking.DAL.DTO;
using CoCoCoWorking.BLL.Models;


namespace CoCoCoWorking.BLL
{
    public class ModelController
    {
        private FinanceReportManager financeReportManager = new FinanceReportManager();
        private RoomManager roomManager = new RoomManager();
        private OrderUnitManager orderUnitManager = new OrderUnitManager();
        private WorkplaceManager workplaceManager = new WorkplaceManager();
        private CustomerManager customerManager = new CustomerManager();
        private AdditionalServiceManager additionalServiceManager = new AdditionalServiceManager();
        private OrderManager orderManager = new OrderManager();
        private RentPriceManager rentPriceManager = new RentPriceManager();
        private AutoMapper.Mapper mapper = MapperConfigStorage.GetInstance();
        private DataStorage _instance = DataStorage.GetInstance();

        private IModelController _controller;

        public ModelController(IModelController controller)
        {
            _controller = controller;
        }

        public ModelController()
        {

        }

        public string GetProductName(FinanceReportDto f)
        {
            string s = "";
            if (f.RoomName != null) 
            {
                s = f.RoomName;
            }
            else if (f.AdditionalServiceName != null)
            {
                s = f.AdditionalServiceName;
            }
            else
            {
                s = "WorkPlaces";
            }
            return s;
        }

        public int GetProductCount(FinanceReportDto f)
        {
            int i = 0;
            if (f.RoomCount != 0)
            {
                i = (int)f.RoomCount;
            }
            else if (f.AdditionalServiceCount != 0)
            {
                i = (int)f.AdditionalServiceCount;
            }
            else if (f.WorkPlaceCount != 0)
            {
                i = (int)f.WorkPlaceCount;
            }
            return i;
        }

        public TypeOfProduct GetTypeOfProduct(RoomDto r)
        {
            TypeOfProduct type = (TypeOfProduct)Enum.Parse(typeof(TypeOfProduct), r.Type);

            return type;
        }
        public string GetTypeOfProduct(RoomModel r)
        {
            string type = r.Type.ToString();

            return type;
        }
        public List<FinanceReportModel> GetFinanceReportModels(DateTime startDate, DateTime endDate)
        {
            List<FinanceReportDto> listDto = financeReportManager.GetFinanceReport(startDate, endDate);
            List<FinanceReportModel> list = mapper.Map<List<FinanceReportModel>>(listDto);
            return list;
        }

        //
        public List<FinanceReportByCustomerModel> GetFinanceReportByCustomerModels(DateTime startDate, DateTime endDate)
        {
            List<FinanceReportByCustomerDto> listDto = financeReportManager.GetFinanceReportByCustomer(startDate, endDate);
            List<FinanceReportByCustomerModel> list = mapper.Map<List<FinanceReportByCustomerModel>>(listDto);
            return list;
        }
        //
        public List<RoomModel> GetAllRoom()
        {
            List<RoomDto> listDto = roomManager.GetAllRooms();
            List<RoomModel> list = mapper.Map<List<RoomModel>>(listDto);
            return list;
        }
        //
        public List<OrderUnitModel> GetAllOrderUnit()
        {
            List<OrderUnitDto> listDto = orderUnitManager.GetAllOrderUnits();
            List<OrderUnitModel> list = mapper.Map<List<OrderUnitModel>>(listDto);
            return list;
        }
        //
        public List<WorkPlaceModel> GetAllWorkplace()
        {
            List<WorkPlaceDto> listDto = workplaceManager.GetAllWorkplaces();
            List<WorkPlaceModel> list = mapper.Map<List<WorkPlaceModel>>(listDto);
            return list;
        }
        //
        public List<AdditionalServiceModel> GetAllAdditionalService()
        {
            List<AdditionalServiceModel> list = new List<AdditionalServiceModel>();
            List<AdditionalServiceDto> listDto = additionalServiceManager.GetAllAdditionalServices();
            list = mapper.Map<List<AdditionalServiceModel>>(listDto);
            return list;
        }

        public List<CustomerModel> GetCustomerWithTheMatchedNumberIsReturned(string v, List<CustomerModel> Cg)
        {
            var d = new List<CustomerModel>();
            foreach (var customermodel in Cg)
            {
                if (customermodel.PhoneNumber.Contains(v))
                {
                    d.Add(customermodel);
                }
            }
            return d;
        }
        public bool IsRegular(CustomersWithOrdersDto customer)
        {
            if (customer.Orders == null || customer.Orders.Count == 0)
            {
                return false;
            }
            foreach (OrderDto order in customer.Orders)
            {
                if (order != null)
                {
                    foreach (OrderUnitDto orderUnit in order.OrderUnits!)
                    {
                        if ((DateTime.Parse(orderUnit.EndDate!) - DateTime.Parse(orderUnit.StartDate!)).Days > 30)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool IsSubscribe(CustomersWithOrdersDto customer)
        {
            if (customer.Orders == null || customer.Orders.Count == 0)
            {
                return false;
            }
            foreach (OrderDto order in customer.Orders)
            {
                if (order != null)
                {
                    foreach (OrderUnitDto orderUnit in order.OrderUnits!)
                    {
                        if (orderUnit.AdditionalServiceId == 1 && DateTime.Parse(orderUnit.EndDate!) > DateTime.Now.Date)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public string GetLastDate(CustomersWithOrdersDto customer)
        {
            DateTime lastDate = new DateTime();
            if (customer.Orders != null)
            {
                foreach (OrderDto order in customer.Orders)
                {
                    if (order != null)
                    {
                        foreach (OrderUnitDto orderUnit in order.OrderUnits!)
                        {
                            DateTime crnt = DateTime.Parse(orderUnit.EndDate);
                            if (crnt > lastDate)
                            {
                                lastDate = crnt;
                            }
                        }
                    }
                }
            }
            if (lastDate == new DateTime())
            {
                return "No orders";
            }
            return lastDate.ToString();
        }
        //
        public void AddCustomerToBase(string firstName, string lastName, string phone, string email)
        {
            CustomerModel customer = new CustomerModel() {FirstName = firstName, LastName = lastName, PhoneNumber = phone, Email = email};
            CustomersWithOrdersDto customerDto = mapper.Map<CustomersWithOrdersDto>(customer);
            customerManager.AddCustomer(customerDto);
        }
        //
        public void UpdateCustomerInBase(CustomerModel customer)
        {            
            CustomersWithOrdersDto customerDto = mapper.Map<CustomersWithOrdersDto>(customer);
            customerManager.UpdateCustomer(customerDto);
        }

        public void AddAditionalService(AdditionalServiceModel service)
        {
            AdditionalServiceDto serviceDto = mapper.Map<AdditionalServiceDto>(service);
            additionalServiceManager.AddAdditionalService(serviceDto);
        }

        public decimal GetSumOrderUnits(List<OrderUnitModel> unitOrders)
        {
            return unitOrders.Sum(unit => unit.OrderUnitCost);
        }
        //
        public string AddOrderInBase(OrderModel order)
        {
            OrderManager orderManager = new OrderManager();
            OrderDto orderDto = mapper.Map<OrderDto>(order);
            var idEnd = orderManager.AddOrder(orderDto);
            return idEnd;
        }
        //
        public AdditionalServiceModel GetAditionalServiceById(int serviceId)
        {
            AdditionalServiceDto additionalServiceDto = additionalServiceManager.GetAdditionalServiceByID(serviceId);
            AdditionalServiceModel additionalServiceModel = mapper.Map<AdditionalServiceModel>(additionalServiceDto);
            return additionalServiceModel;
        }
        //
        public void UpdateAdditionalService(AdditionalServiceModel additionalService)
        {
            AdditionalServiceDto additionalServiceDto = mapper.Map<AdditionalServiceDto>(additionalService);
            additionalServiceManager.UpdateAdditionalService(additionalServiceDto);
        }
        //
        public List<OrderModel> GetOrderByCustomerID(int id)
        {
            List<OrderDto> listDto = orderManager.OrderGetByCustomerId(id);
            List<OrderModel> order = mapper.Map<List<OrderModel>>(listDto);
            return order;
        }
        //
        public void AddUnitOrdertoBase(OrderUnitModel orderUnit)
        {
            OrderUnitManager orderUnitManager = new OrderUnitManager();
            OrderUnitDto orderDto = mapper.Map<OrderUnitDto>(orderUnit);
            orderUnitManager.AddOrderUnit(orderDto);
        }
        //
        public void DeleteAdditionalService(int serviceId)
        {
            additionalServiceManager.DeleteAdditionalService(serviceId);
        }
        //
        public void AddRoom(RoomModel room)
        {
            RoomDto roomDto = mapper.Map<RoomDto>(room);
            roomManager.AddRoom(roomDto);
        }

        public void AddRentPrice(RentPriceModel price)
        {
            RentPriceDto priceDto = mapper.Map<RentPriceDto>(price);
            rentPriceManager.AddRentPrice(priceDto);
        }

        public void DeleteRentPrice(int id)
        {
            rentPriceManager.DeleteRentPrice(id);
        }
    }  
}