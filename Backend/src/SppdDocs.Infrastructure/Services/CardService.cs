﻿using System;
using System.Collections.Generic;
using System.Linq;
using SppdDocs.Core.Domain.Entities;
using SppdDocs.Core.Domain.Repositories;
using SppdDocs.Core.Services;

namespace SppdDocs.Infrastructure.Services
{
	internal class CardService : ICardService
	{
		private readonly ICardRepository _cardRepository;

		public CardService(ICardRepository cardRepository)
		{
			_cardRepository = cardRepository;
		}

		public Card GetCurrent(Guid cardId)
		{
			return _cardRepository.GetCardsFull()
			                      .SingleOrDefault(card => card.EntityId == cardId);
		}

		public IEnumerable<Card> GetAllCurrent()
		{
			return _cardRepository.GetCardsFull().ToList();
		}
	}
}