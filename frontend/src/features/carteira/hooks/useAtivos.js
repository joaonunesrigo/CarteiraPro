import { useState } from 'react'
import { carteiraApi } from '../services/carteira.api'
import {
  formularioTemErros,
  validarFormularioAtivo,
} from '../utils/validarFormularioAtivo'
import { normalizarTicker } from '../utils/validarTicker'

const formularioInicial = {
  ticker: '',
  precoMedio: '',
  quantidade: '',
  tipo: 0,
}

const errosIniciais = {
  ticker: null,
  precoMedio: null,
  quantidade: null,
  tipo: null,
}

export function useAdicionarAtivo(
  aoConcluir,
  tickersCadastrados = [],
  mostrarToast,
) {
  const [formulario, setFormulario] = useState(formularioInicial)
  const [errosCampos, setErrosCampos] = useState(errosIniciais)
  const [enviando, setEnviando] = useState(false)

  function atualizarCampo(campo) {
    return (evento) => {
      setFormulario((atual) => ({ ...atual, [campo]: evento.target.value }))
      setErrosCampos((atual) => ({ ...atual, [campo]: null }))
    }
  }

  async function validarTickerExterno(ticker) {
    try {
      await carteiraApi.obterCotacao(ticker)
      return null
    } catch {
      return `O ticker ${ticker} não foi encontrado. Verifique o símbolo e tente novamente.`
    }
  }

  async function enviarFormulario(evento) {
    evento.preventDefault()
    setEnviando(true)

    const erros = validarFormularioAtivo(formulario, tickersCadastrados)

    if (formularioTemErros(erros)) {
      setErrosCampos((atual) => ({ ...atual, ...erros }))
      setEnviando(false)
      return
    }

    const ticker = normalizarTicker(formulario.ticker)
    const erroBrapi = await validarTickerExterno(ticker)

    if (erroBrapi) {
      setErrosCampos((atual) => ({ ...atual, ticker: erroBrapi }))
      setEnviando(false)
      return
    }

    try {
      await carteiraApi.adicionarAtivo({
        ticker,
        precoMedio: Number(formulario.precoMedio),
        quantidade: Number(formulario.quantidade),
        tipo: Number(formulario.tipo),
      })
      setFormulario(formularioInicial)
      setErrosCampos(errosIniciais)
      mostrarToast(`${ticker} adicionado à carteira.`, 'sucesso')
      aoConcluir()
    } catch (err) {
      const mensagem = err.message || 'Erro ao adicionar ativo'
      if (
        mensagem.toLowerCase().includes('ticker') ||
        mensagem.toLowerCase().includes('encontrado') ||
        mensagem.toLowerCase().includes('cadastrado')
      ) {
        setErrosCampos((atual) => ({ ...atual, ticker: mensagem }))
      } else {
        mostrarToast(mensagem, 'erro')
      }
    } finally {
      setEnviando(false)
    }
  }

  return {
    formulario,
    errosCampos,
    atualizarCampo,
    enviando,
    enviarFormulario,
  }
}

export function useExcluirAtivo(aoConcluir, mostrarToast, solicitarConfirmacao) {
  async function excluirAtivo(id, ticker) {
    if (!id) return

    const confirmou = await solicitarConfirmacao({
      titulo: 'Remover ativo',
      mensagem: `Deseja remover ${ticker} da carteira? Esta ação não pode ser desfeita.`,
      textoConfirmar: 'Remover',
      textoCancelar: 'Cancelar',
      variante: 'perigo',
    })

    if (!confirmou) return

    try {
      await carteiraApi.removerAtivo(id)
      mostrarToast?.(`${ticker} foi removido da carteira.`, 'sucesso')
      aoConcluir()
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao remover ativo', 'erro')
    }
  }

  async function excluirTodosAtivos(quantidade) {
    if (!quantidade) return

    const confirmou = await solicitarConfirmacao({
      titulo: 'Remover todos os ativos',
      mensagem: `Deseja remover os ${quantidade} ativos da carteira? Esta ação não pode ser desfeita.`,
      textoConfirmar: 'Remover todos',
      textoCancelar: 'Cancelar',
      variante: 'perigo',
    })

    if (!confirmou) return

    try {
      const resultado = await carteiraApi.removerTodosAtivos()
      const removidos = resultado?.removidos ?? quantidade
      mostrarToast?.(
        `${removidos} ativo(s) removido(s) da carteira.`,
        'sucesso',
      )
      aoConcluir()
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao remover ativos', 'erro')
    }
  }

  return { excluirAtivo, excluirTodosAtivos }
}
