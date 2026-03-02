from playwright.sync_api import sync_playwright

def verify():
    with sync_playwright() as p:
        browser = p.chromium.launch()
        page = browser.new_page()
        page.goto('http://localhost:5056/Eventi/Partecipa')

        # Wait for any potential transitions
        page.wait_for_timeout(2000)

        # Capture screenshot
        page.screenshot(path='/home/jules/verification/partecipa_page_updated.png', full_page=True)

        browser.close()

if __name__ == '__main__':
    verify()
