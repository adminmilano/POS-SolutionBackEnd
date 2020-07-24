using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
namespace Milano.BackEnd.Dto
{
	/// <summary>
	/// Clase de usuaurio que viaja como petición
	/// </summary>
	[DataContract]
	public class UserRequest
	{
		/// <summary>
		/// Numero de empleado
		/// </summary>
		[DataMember(Name = "numberEmployee")]
		public int NumberEmployee { get; set; }
		/// <summary>
		/// Contraseña del empleado
		/// </summary>
		[DataMember(Name = "password")]
		public string Password { get; set; }
		/// <summary>
		/// Numero de intentos en el login
		/// </summary>
		[DataMember(Name = "numberAttempts")]
		public int NumberAttempts { get; set; }
    }
}
