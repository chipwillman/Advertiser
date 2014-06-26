Feature: ClaimEstablishment
	In order to allow access to edit establishment details
	As an establishment owner
	I want to be claim an establishment as mine

Scenario: Claim unclaimed establishment
	Given I have a wwdrink account 
	And I have located an unclaimed establishmnet
	When I claim an establishment as mine
	Then I should be allowed to edit the establishment details

