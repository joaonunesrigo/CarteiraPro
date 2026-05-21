import { useImportarProventosMutation } from '../mutations/proventosMutations'
import { proventosApi } from '../services/proventos.api'
import { useProventosStore } from '../stores/proventosStore'

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

export function useImportarMovimentacaoB3(mostrarToast) {
  const importacao = useProventosStore((state) => state.importacao)
  const setImportacao = useProventosStore((state) => state.setImportacao)
  const alternarSelecionadoImportacao = useProventosStore((state) => state.alternarSelecionadoImportacao)
  const limparImportacao = useProventosStore((state) => state.limparImportacao)
  const importarProventos = useImportarProventosMutation()
  const { linhas, processandoArquivo, erroArquivo, nomeArquivo } = importacao

  async function aoSelecionarArquivo(evento) {
    const arquivo = evento.target.files?.[0]
    evento.target.value = ''

    if (!arquivo) return

    setImportacao({
      erroArquivo: null,
      processandoArquivo: true,
      nomeArquivo: arquivo.name,
    })

    try {
      const resposta = await proventosApi.previewImportacaoB3(arquivo)
      const preview = (resposta.linhas ?? []).map((dados, indice) => criarLinhaPreview(dados, indice))

      setImportacao({ linhas: preview })

      if (preview.length === 0) {
        setImportacao({ erroArquivo: 'Nenhum provento encontrado no arquivo.' })
      }
    } catch (err) {
      setImportacao({
        linhas: [],
        erroArquivo: err.message || 'Erro ao ler o arquivo.',
      })
    } finally {
      setImportacao({ processandoArquivo: false })
    }
  }

  function alternarSelecionado(id) {
    alternarSelecionadoImportacao(id)
  }

  function limparPreview() {
    limparImportacao()
  }

  async function confirmarImportacao() {
    const selecionadas = linhas.filter((linha) => linha.selecionado && !linha.jaImportado)

    if (selecionadas.length === 0) {
      mostrarToast?.('Selecione ao menos um provento para importar.', 'erro')
      return
    }

    try {
      const resultado = await importarProventos.mutateAsync({
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
      }
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao importar proventos.', 'erro')
    }
  }

  const totalSelecionadas = linhas.filter((linha) => linha.selecionado && !linha.jaImportado).length

  return {
    linhas,
    processandoArquivo,
    importando: importarProventos.isPending,
    erroArquivo,
    nomeArquivo,
    totalSelecionadas,
    aoSelecionarArquivo,
    alternarSelecionado,
    limparPreview,
    confirmarImportacao,
  }
}
