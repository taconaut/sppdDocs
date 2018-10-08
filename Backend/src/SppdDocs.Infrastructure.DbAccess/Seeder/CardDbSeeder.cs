﻿using System;
using SppdDocs.Core.Domain.Entities;
using SppdDocs.Core.Domain.Repositories;

namespace SppdDocs.Infrastructure.DbAccess.Seeder
{
	internal class CardDbSeeder : IDbSeeder
	{
		private readonly IRepository<Card> _cardRepository;

		public CardDbSeeder(IRepository<Card> cardRepository)
		{
			_cardRepository = cardRepository;
		}

		public int Priority => 100;

		public void Seed()
		{
			_cardRepository.Add(new Card
			                    {
				                    EntityId = new Guid(SeederConstants.Card.STAN_OF_MANY_MOONS_ENTITY_ID),
				                    IsCurrent = true,
				                    Name = "Stan of Many Moons",
				                    Description =
					                    "Stan of many Moons is a 4-cost fighter from the Adventure theme who is available at rank 5 that deals damage to all enemy units on the battlefield with his effect.",
				                    RarityId = new Guid(SeederConstants.Rarity.LEGENDARY_ID),
				                    ClassId = new Guid(SeederConstants.CardClass.FIGHTER_ID)
			                    });
		}
	}
}