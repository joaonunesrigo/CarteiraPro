import { useState } from 'react'
import { carteiraApi } from '../services/carteira.api'

function dataHoje() {
  return new Date().toISOString().slice(0, 10)
}

const formularioInicial = {
  tipo: 0,
  data: dataHoje(),
  quantidade: '',
  precoUnitario: '',
  taxas: '',
  observacao: '',
}

export function useOperacoesAtivo(
  aoAtualizarCarteira,
  mostrarToast,
  solicitarConfirmacao,
) {
  const [ativoSelecionado, setAtivoSelecionado] = useState(null)
  const [operacoes, setOperacoes] = useState([])
  const [formulario, setFormulario] = useState(formularioInicial)
  const [carregando, setCarregando] = useState(false)
  const [salvando, setSalvando] = useState(false)

  async function carregarOperacoes(ativo) {
    setCarregando(true)
    try {
      const dados = await carteiraApi.listarOperacoes(ativo.id)
      setOperacoes(dados ?? [])
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao carregar operações', 'erro')
    } finally {
      setCarregando(false)
    }
  }

  async function abrirPainel(ativo) {
    setAtivoSelecionado(ativo)
    setFormulario(formularioInicial)
    await carregarOperacoes(ativo)
  }

  function fecharPainel() {
    setAtivoSelecionado(null)
    setOperacoes([])
    setFormulario(formularioInicial)
  }

  function atualizarCampo(campo) {
    return (evento) => {
      setFormulario((atual) => ({ ...atual, [campo]: evento.target.value }))
    }
  }

  async function adicionarOperacao(evento) {
    evento.preventDefault()
    if (!ativoSelecionado) return

    setSalvando(true)
    try {
      await carteiraApi.adicionarOperacao(ativoSelecionado.id, {
        tipo: Number(formulario.tipo),
        data: formulario.data,
        quantidade: Number(formulario.quantidade),
        precoUnitario: Number(formulario.precoUnitario),
        taxas: Number(formulario.taxas || 0),
        observacao: formulario.observacao || null,
      })

      setFormulario(formularioInicial)
      await carregarOperacoes(ativoSelecionado)
      await aoAtualizarCarteira?.()
      mostrarToast?.('Operação registrada.', 'sucesso')
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao registrar operação', 'erro')
    } finally {
      setSalvando(false)
    }
  }

  async function removerOperacao(operacao) {
    const confirmou = await solicitarConfirmacao?.({
      titulo: 'Remover operação',
      mensagem:
        'Deseja remover esta operação? A posição do ativo será recalculada.',
      textoConfirmar: 'Remover',
      textoCancelar: 'Cancelar',
      variante: 'perigo',
    })

    if (!confirmou) return

    try {
      await carteiraApi.removerOperacao(operacao.id)
      if (ativoSelecionado) {
        await carregarOperacoes(ativoSelecionado)
      }
      await aoAtualizarCarteira?.()
      mostrarToast?.('Operação removida.', 'sucesso')
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao remover operação', 'erro')
    }
  }

  return {
    ativoSelecionado,
    operacoes,
    formulario,
    carregando,
    salvando,
    abrirPainel,
    fecharPainel,
    atualizarCampo,
    adicionarOperacao,
    removerOperacao,
  }
}
