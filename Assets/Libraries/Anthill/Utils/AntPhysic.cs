namespace Anthill.Utils
{
	using UnityEngine;
	
	public static class AntPhysic
	{
		/// <summary>
		/// Применяет силу взрыва к указанному телу.
		/// </summary>
		/// <param name="aBody">Тело к которому применяется сила.</param>
		/// <param name="aPosition">Точка взрыва.</param>
		/// <param name="aForce">Сила взрыва.</param>
		/// <param name="aRadius">Радиус взрыва.</param>
		/// <returns></returns>
		public static void AddExplosionForce(Rigidbody2D aBody, Vector3 aPosition, float aForce, float aRadius)
		{
			var dir = aBody.transform.position - aPosition;
			float calc = 1.0f - (dir.magnitude / aRadius);
			calc = (calc <= 0.0f) ? 0.0f : calc;
			aBody.AddForce(dir.normalized * aForce * calc);
		}

		/// <summary>
		/// Рассчитывает силу при столкновении двух физических тел.
		/// </summary>
		/// <param name="aBody">Движущееся тело.</param>
		/// <param name="aCollider">Препятствие с которым произошло столкновение.</param>
		/// <returns>Сила удара.</returns>
		public static float GetCollisionForce(Rigidbody2D aBody, Collider2D aCollider)
		{
			float impactVelocityX = aBody.velocity.x;
			float impactVelocityY = aBody.velocity.y;
			float impactVelocity;
			float impactForce;
			float impactMass = 1.0f;

			if (aCollider.attachedRigidbody != null)
			{
				impactVelocityX -= aCollider.attachedRigidbody.velocity.x;
				impactVelocityY -= aCollider.attachedRigidbody.velocity.y;
				impactMass = aCollider.attachedRigidbody.mass;
			}

			impactVelocityX *= Mathf.Sign(impactVelocityX);
			impactVelocityY *= Mathf.Sign(impactVelocityY);
			impactVelocity = impactVelocityX * impactVelocityY;
			impactForce = impactVelocity * aBody.mass * impactMass;
			impactForce *= Mathf.Sign(impactForce);

			return impactForce;

			/*var impactVelocityX = rigidbody.velocity.x - contact.otherCollider.rigidbody.velocity.x;
			impactVelocityX *= Mathf.Sign(impactVelocityX);
			var impactVelocityY = rigidbody.velocity.y - contact.otherCollider.rigidbody.velocity.y;
			impactVelocityY *= Mathf.Sign(impactVelocityY);
			var impactVelocity = impactVelocityX + impactVelocityY;
			var impactForce = impactVelocity * rigidbody.mass * contact.otherCollider.rigidbody.mass;
			impactForce *= Mathf.Sign(impactForce);*/
		}

		// Проверить попадание точки внутрь тела:
		// Collider2D collider.bounds.Contains(node.stats.ImpactBullet.ImpactPoint)
	}
}