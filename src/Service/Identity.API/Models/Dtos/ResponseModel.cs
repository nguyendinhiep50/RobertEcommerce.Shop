﻿namespace Identity.API.Models.Dtos;

public class ResponseModel<T>
{
	public bool IsSuccess { get; set; }
	public string? Message { get; set; }
	public T? Data { get; set; }
}