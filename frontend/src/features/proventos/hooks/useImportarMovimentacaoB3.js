import { useState } from 'react'
import { proventosApi } from '../services/proventos.api'

function criarLinhaPreview(dados, indice) {
  return {
    id: `${dados.ticker}-${dados.dataPagamento}-${indice}`,
    ticker: dados.ticker,
    produto: dados.produto,
    movimentacao: dados.movimentacao,
    quantidade: dados.quantidade,
    valorPorCota: dados.valorPorCota,
    valorTotal: dados.valorTotal,
    dataPagamento: dados.dataPagamento,
    tipo: dados.tipo,
    ativoCadastrado: dados.ativoCadastrado,
    jaImportado: dados.jaImportado,
    origemAba: dados.origemAba,
    selecionado: !dados.jaImportado,
  }
}

export function useImportarMovimentacaoB3(recarregar, mostrarToast) {
  const [linhas, setLinhas] = useState([])
  const [processandoArquivo, setProcessandoArquivo] = useState(false)
  const [importando, setImportando] = useState(false)
  const [erroArquivo, setErroArquivo] = useState(null)
  const [nomeArquivo, setNomeArquivo] = useState(null)

  async function aoSelecionarArquivo(evento) {
    const arquivo = evento.target.files?.[0]
    evento.target.value = ''

    if (!arquivo) return

    setErroArquivo(null)
    setProcessandoArquivo(true)
    setNomeArquivo(arquivo.name)

    try {
      const resposta = await proventosApi.previewImportacaoB3(arquivo)
      const preview = (resposta.linhas ?? []).map((dados, indice) =>
        criarLinhaPreview(dados, indice),
      )

      setLinhas(preview)

      if (preview.length === 0) {
        setErroArquivo('Nenhum provento encontrado no arquivo.')
      }
    } catch (err) {
      setLinhas([])
      setErroArquivo(err.message || 'Erro ao ler o arquivo.')
    } finally {
      setProcessandoArquivo(false)
    }
  }

  function alternarSelecionado(id) {
    setLinhas((atual) =>
      atual.map((linha) =>
        linha.id === id && !linha.jaImportado
          ? { ...linha, selecionado: !linha.selecionado }
          : linha,
      ),
    )
  }

  function limparPreview() {
    setLinhas([])
    setErroArquivo(null)
    setNomeArquivo(null)
  }

  async function confirmarImportacao() {
    const selecionadas = linhas.filter(
      (linha) => linha.selecionado && !linha.jaImportado,
    )

    if (selecionadas.length === 0) {
      mostrarToast?.('Selecione ao menos um provento para importar.', 'erro')
      return
    }

    setImportando(true)

    try {
      const resultado = await proventosApi.importar({
        ignorarDuplicados: true,
        proventos: selecionadas.map((linha) => ({
          ticker: linha.ticker,
          valorPorCota: linha.valorPorCota,
          quantidade: linha.quantidade,
          dataPagamento: linha.dataPagamento,
          tipo: linha.tipo,
        })),
      })

      const { importados, ignorados, erros } = resultado
      const partes = []

      if (importados > 0) partes.push(`${importados} importado(s)`)
      if (ignorados > 0) partes.push(`${ignorados} ignorado(s)`)
      if (erros?.length > 0) partes.push(`${erros.length} erro(s)`)

      mostrarToast?.(partes.join(', ') || 'Importação concluída.', 'sucesso')

      if (importados > 0) {
        limparPreview()
        recarregar()
      }
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao importar proventos.', 'erro')
    } finally {
      setImportando(false)
    }
  }

  const totalSelecionadas = linhas.filter(
    (linha) => linha.selecionado && !linha.jaImportado,
  ).length

  return {
    linhas,
    processandoArquivo,
    importando,
    erroArquivo,
    nomeArquivo,
    totalSelecionadas,
    aoSelecionarArquivo,
    alternarSelecionado,
    limparPreview,
    confirmarImportacao,
  }
}
