Feature: List Specials
	In order to promote my business
	As an establishment owner
	I want to be able to show my specials to customers

@mytag
Scenario: List happy hour specials
	Given I have claimed an establishment
	And I have elected to list happy hour specials
	When I search near my establishment
	Then I should see my happy hour specials 
