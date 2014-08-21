package com.youbetme.androidsample.dataobjects;

public enum BetStatus
{
	Pending, // Default initial state.
	Active,
	Payup,
	Paid,
	Rejected,
	InProgress, // This is the initial state when creating a new bet using SMS system.
	Welched
}