using System;
using Sandbox;

namespace Kicks
{
	public interface IHealthComponent
	{
		public float MaxHealth { get; }
		public float Health { get; }
		public void TakeDamage(float damage, PlayerController attacker);
	}
}
