﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TomMotos.Conexao;
using TomMotos.Model;

namespace TomMotos.view
{
    public partial class FmrFinalizar_Orcamento : Form
    {
        Fmrcaixa Fno;
        public FmrFinalizar_Orcamento(Fmrcaixa Fnoo)
        {
            InitializeComponent();
            Fno = Fnoo;
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            txtFaltaPagar.Text = "";
            txtValorPago.Text = (double.Parse(txtDinheiro.Text.Replace(".", ",")) + double.Parse(txtDebito.Text.Replace(".", ",")) + double.Parse(txt_credito.Text.Replace(".", ",")) + double.Parse(txtPix.Text.Replace(".", ","))).ToString();
            if (double.Parse(txtValorPago.Text) >= double.Parse(lblsubtotal.Text))
            {
                txtValorPago.Text = "";
                MessageBox.Show("ADIANTAMENTO NÃO PODE SER MAIOR QUE O TOTAL EM ORÇAMENTO", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else txtFaltaPagar.Text = (double.Parse(lblsubtotal.Text) - double.Parse(txtValorPago.Text)).ToString();
        }
        private void carregarEmails()
        {
            try
            {
                if (CaixaModel.fk_cliente_id != null)
                {
                    MySqlConnection conexao = ConnectionFactory.getConnection();
                    MySqlConnection cn = new MySqlConnection();
                    cn = conexao;
                    cn.Open();
                    MySqlCommand com = new MySqlCommand();
                    com.Connection = cn;
                    com.CommandText = "select nome_email from tb_usuario inner join tb_email on tb_usuario.id_usuario = tb_email.fk_usuario_id inner join tb_cliente on tb_cliente.id_cliente = tb_usuario.fk_cliente_id where fk_cliente_id = " + CaixaModel.fk_cliente_id;
                    MySqlDataReader dr = com.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    cbxEmails.DisplayMember = "nome_email";
                    cbxEmails.DataSource = dt;
                    cbxEmails.Text = null;
                }
            }
            catch (Exception erro)
            { MessageBox.Show("" + erro.Message); }

        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            CaixaModel.emailCliente = cbxEmails.Text;
            MessageBox.Show("Test "+ cbxEmails.Text);
            try
            {
                if (cbxEmails.Text == "")
                {
                    var result = MessageBox.Show("Deseja enviar Comprovante no email? ", "Orçamento",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        MessageBox.Show("Escolha o Email que deseja enviar!");
                        return;
                    }
                    else
                    {
                        CaixaModel.valor_pago = double.Parse(txtValorPago.Text);
                        CaixaModel.eOrcamento = true;
                        Fno.finalizarOrcamento();                        
                        Fno.SalvarPdf();
                        Fno.finalizarFormCaixa(true);
                    }
                }
                else
                {
                    CaixaModel.valor_pago = double.Parse(txtValorPago.Text);
                    CaixaModel.eOrcamento = true;
                    Fno.finalizarOrcamento();
                    Fno.SalvarPdf();
                    Fno.EnviarEmail();
                    Fno.finalizarFormCaixa(true);
                }
                this.Close();
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro " + erro.Message);
            }

        }

        private void FmrFinalizar_Orcamento_Load(object sender, EventArgs e)
        {
            lblsubtotal.Text = Fno.lblSubitotal.Text;
            carregarEmails();
        }
    }
}
