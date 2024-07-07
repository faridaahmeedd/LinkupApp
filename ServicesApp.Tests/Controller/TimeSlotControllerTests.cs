using Xunit;
using FakeItEasy;
using AutoMapper;
using FluentAssertions;
using System.Collections.Generic;
using ServicesApp.Interfaces;
using ServicesApp.Dto.Service;
using ServicesApp.Models;
using ServicesApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Helper;

namespace ServicesApp.Tests.Controller
{
	public class TimeSlotControllerTests
	{
		private readonly ITimeSlotsRepository _timeSlotRepository;
		private readonly IServiceRequestRepository _requestRepository;
		private readonly IMapper _mapper;
		private readonly TimeSlotController _controller;

		public TimeSlotControllerTests()
		{
			_timeSlotRepository = A.Fake<ITimeSlotsRepository>();
			_requestRepository = A.Fake<IServiceRequestRepository>();
			_mapper = A.Fake<IMapper>();
			_controller = new TimeSlotController(_timeSlotRepository, _requestRepository, _mapper);
		}

		[Fact]
		public void GetTimeSlot_TimeSlotExists_ReturnsOk()
		{
			// Arrange
			var timeSlotId = 1;
			var timeSlot = A.Fake<TimeSlot>();
			var timeSlotDto = A.Fake<TimeSlotDto>();
			A.CallTo(() => _timeSlotRepository.TimeSlotExist(timeSlotId)).Returns(true);
			A.CallTo(() => _timeSlotRepository.GetTimeSlot(timeSlotId)).Returns(timeSlot);
			A.CallTo(() => _mapper.Map<TimeSlotDto>(timeSlot)).Returns(timeSlotDto);

			// Act
			var result = _controller.GetTimeSlot(timeSlotId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetTimeSlot_TimeSlotDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var timeSlotId = 1;
			A.CallTo(() => _timeSlotRepository.TimeSlotExist(timeSlotId)).Returns(false);

			// Act
			var result = _controller.GetTimeSlot(timeSlotId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetTimeSlotsOfService_ServiceExists_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var timeSlots = A.Fake<ICollection<TimeSlot>>();
			var timeSlotDtos = A.Fake<List<TimeSlotDto>>();
			A.CallTo(() => _requestRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _timeSlotRepository.GetTimeSlotsOfService(serviceId)).Returns(timeSlots);
			A.CallTo(() => _mapper.Map<List<TimeSlotDto>>(timeSlots)).Returns(timeSlotDtos);

			// Act
			var result = _controller.GetTimeSlotsOfService(serviceId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetTimeSlotsOfService_ServiceDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _requestRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.GetTimeSlotsOfService(serviceId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void AddTimeSlots_ValidTimeSlots_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var timeslot = A.Fake<TimeSlot>();
			var timeSlotDtos = A.Fake<ICollection<TimeSlotDto>>();
			var serviceRequest = A.Fake<ServiceRequest>();
			A.CallTo(() => _requestRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _requestRepository.GetService(serviceId)).Returns(serviceRequest);
			A.CallTo(() => _mapper.Map<TimeSlot>(A<TimeSlotDto>.Ignored)).ReturnsLazily((TimeSlotDto dto) =>
			{
				var slot = timeslot;
				slot.ServiceRequest = serviceRequest;
				return slot;
			});


			// Act
			var result = _controller.AddTimeSlots(serviceId, timeSlotDtos);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void AddTimeSlots_TimeSlotsExceedMax_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _requestRepository.ServiceExist(serviceId)).Returns(true);
			var timeSlotDtos = new List<TimeSlotDto> { A.Fake<TimeSlotDto>(), A.Fake<TimeSlotDto>(), A.Fake<TimeSlotDto>(), A.Fake<TimeSlotDto>() };

			// Act
			var result = _controller.AddTimeSlots(serviceId, timeSlotDtos);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public void AddTimeSlots_ServiceDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			var timeSlotDtos = A.Fake<ICollection<TimeSlotDto>>();
			A.CallTo(() => _requestRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.AddTimeSlots(serviceId, timeSlotDtos);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void UpdateTimeSlots_ValidTimeSlots_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var timeslot = A.Fake<TimeSlot>();
			var timeSlotDtos = A.Fake<ICollection<TimeSlotDto>>();
			var serviceRequest = A.Fake<ServiceRequest>();
			A.CallTo(() => _requestRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _requestRepository.GetService(serviceId)).Returns(serviceRequest);
			A.CallTo(() => _mapper.Map<TimeSlot>(A<TimeSlotDto>.Ignored)).ReturnsLazily((TimeSlotDto dto) =>
			{
				var slot = timeslot;
				slot.ServiceRequest = serviceRequest;
				return slot;
			});

			// Act
			var result = _controller.UpdateTimeSlots(serviceId, timeSlotDtos);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void UpdateTimeSlots_TimeSlotsExceedMax_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var timeSlotDtos = new List<TimeSlotDto> { A.Fake<TimeSlotDto>(), A.Fake<TimeSlotDto>(), A.Fake<TimeSlotDto>(), A.Fake<TimeSlotDto>() };

			// Act
			var result = _controller.UpdateTimeSlots(serviceId, timeSlotDtos);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public void UpdateTimeSlots_ServiceDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			var timeSlotDtos = A.Fake<ICollection<TimeSlotDto>>();
			A.CallTo(() => _requestRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.UpdateTimeSlots(serviceId, timeSlotDtos);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}
	}
}
