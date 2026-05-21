import { Modal } from '../../../../components/Modal'
import { ImportadorB3 } from './ImportadorB3'

export function ModalImportadorB3({ aberto, onFechar, importadorB3 }) {
  return (
    <Modal aberto={aberto} onFechar={onFechar} titulo="Importar da B3" tamanho="xl">
      <ImportadorB3 {...importadorB3} />
    </Modal>
  )
}
