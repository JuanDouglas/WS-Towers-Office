using WSTowersOffice.Api.Models.Enums;

namespace WSTowersOffice.Api.Models
{
    /// <summary>
    /// Modelo de dados sobre IP
    /// </summary>
    public class IPModel
    {
        /// <summary>
        /// ID do IP no banco de dados.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Endereço de IP Cadastrado.
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Indica o nível de confiança do sistema neste IP. 
        /// </summary>
        public string Confiance => EnumConfiance.ToString();
        private Confiance EnumConfiance { get; set; }
        /// <summary>
        /// Indica se este IP já foi banido.
        /// </summary>
        public bool AlreadyBeenBanned { get; set; }

        /// <summary>
        /// Construtor a partir de um IP do Banco de Dados.
        /// </summary>
        /// <param name="ip">Classe 'IP' do Entity Freamework.</param>
        public IPModel(IP ip)
        {
            Id = ip.ID;
            IP = ip.IP1;
            EnumConfiance = (Confiance)ip.Confiance;
        }
    }
}